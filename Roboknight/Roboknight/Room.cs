//Author: Ilan Segal
//File Name: Room.cs
//Project Name: RoboKnight
//Creation Date: December 10, 2015
//Modified Date: January 16, 2016
//Description: Class which represents a single room within the game.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Roboknight
{
    class Room
    {
        //Used for sound loading
        private ContentManager content;

        //Sound effects
        private SoundEffect enemyDeathEffect;
        private SoundEffect sandpileRegenEffect;
        private SoundEffect roomClearEffect;
        
        //Object data
        private List<Obstacle> obstacles;
        private List<Enemy> enemies;
        private List<ItemPedestal> items;
        private Room[] connectedRooms;
        private Shape[] doorHitboxes;
        
        //Keeps track of whether or not this room has been visited
        private bool wasEntered;

        //Constants dealing with wall generation
        private const int DOOR_WIDTH = 150;
        private const int WALL_THICKNESS = 20;
        private const int ROOM_HEIGHT = 675;
        private const int ROOM_WIDTH = 1200;

        //Standard size for the item pedestals
        private const int PEDESTAL_SIZE = 150;

        //Allows for easy standardization of obstacle locations and sizes
        private const int GRID_UNIT = 75;

        //Used when managing rooms' positions relative to eachother
        private const int UP = 0;
        private const int RIGHT = 1;
        private const int DOWN = 2;
        private const int LEFT = 3;

        /// <summary>
        /// Constructor for the Room class
        /// </summary>
        /// <param name="content">A valid ContentManager</param>
        /// <param name="roomID">The ID of the room layout</param>
        public Room(ContentManager content, int roomID)
        {
            this.content = content;

            enemyDeathEffect = this.content.Load<SoundEffect>(@"Sound\Enemy\dead");
            sandpileRegenEffect = this.content.Load<SoundEffect>(@"Sound\Enemy\regen");
            roomClearEffect = this.content.Load<SoundEffect>(@"Sound\Misc\roomclear");

            obstacles = new List<Obstacle>();
            enemies = new List<Enemy>();
            items = new List<ItemPedestal>();
            connectedRooms = new Room[4];
            doorHitboxes = new Shape[4];

            this.GenerateRoom(roomID);

            wasEntered = false;
        }

        /// <summary>
        /// Builds walls.
        /// NOTE: To be used after generating floor, so that doors form properly.
        /// </summary>
        public void BuildWalls()
        {
            List<Obstacle> walls = new List<Obstacle>();

            //Walls on the top side of the room
            if (connectedRooms[UP] != null)
            {
                //Creates opening to move to another room
                walls.Add(new Obstacle(new Rect(Vector2.Zero, ROOM_WIDTH / 2 - DOOR_WIDTH / 2, WALL_THICKNESS), "wall"));
                walls.Add(new Obstacle(new Rect(new Vector2(ROOM_WIDTH / 2 + DOOR_WIDTH / 2, 0), ROOM_WIDTH / 2 - DOOR_WIDTH / 2, WALL_THICKNESS), "wall"));

                //Ensures player does not leave window boundaries, if he is unable to leave the room properly
                obstacles.Add(new Obstacle(new Rect(new Vector2((float)GRID_UNIT * 7, (float)GRID_UNIT * -1), GRID_UNIT, GRID_UNIT), "door"));
                obstacles.Add(new Obstacle(new Rect(new Vector2((float)GRID_UNIT * 8, (float)GRID_UNIT * -1), GRID_UNIT, GRID_UNIT), "door"));
            }
            else
            {
                //If an adjacent room in this direction does not exist, creates a solid wall
                walls.Add(new Obstacle(new Rect(Vector2.Zero, ROOM_WIDTH, WALL_THICKNESS), "wall"));
            }

            //Walls on the right side of the room
            if (connectedRooms[RIGHT] != null)
            {
                //Creates opening to move to another room
                walls.Add(new Obstacle(new Rect(new Vector2(ROOM_WIDTH - WALL_THICKNESS, 0), WALL_THICKNESS, ROOM_HEIGHT / 2 - DOOR_WIDTH / 2), "wall"));
                walls.Add(new Obstacle(new Rect(new Vector2(ROOM_WIDTH - WALL_THICKNESS, ROOM_HEIGHT / 2 + DOOR_WIDTH / 2), WALL_THICKNESS, ROOM_HEIGHT / 2 - DOOR_WIDTH / 2), "wall"));

                //Ensures player does not leave window boundaries, if he is unable to leave the room properly
                obstacles.Add(new Obstacle(new Rect(new Vector2((float)GRID_UNIT * 16, (float)GRID_UNIT * 3), GRID_UNIT, GRID_UNIT), "door"));
                obstacles.Add(new Obstacle(new Rect(new Vector2((float)GRID_UNIT * 16, (float)GRID_UNIT * 4), GRID_UNIT, GRID_UNIT), "door"));
                obstacles.Add(new Obstacle(new Rect(new Vector2((float)GRID_UNIT * 16, (float)GRID_UNIT * 5), GRID_UNIT, GRID_UNIT), "door"));
            }
            else
            {
                //If an adjacent room in this direction does not exist, creates a solid wall
                walls.Add(new Obstacle(new Rect(new Vector2(ROOM_WIDTH - WALL_THICKNESS, 0), WALL_THICKNESS, ROOM_HEIGHT), "wall"));
            }

            //Walls on the bottom side of the room
            if (connectedRooms[DOWN] != null)
            {
                //Creates opening to move to another room
                walls.Add(new Obstacle(new Rect(new Vector2(0, ROOM_HEIGHT - WALL_THICKNESS), ROOM_WIDTH / 2 - DOOR_WIDTH / 2, WALL_THICKNESS), "wall"));
                walls.Add(new Obstacle(new Rect(new Vector2(ROOM_WIDTH / 2 + DOOR_WIDTH / 2, ROOM_HEIGHT - WALL_THICKNESS), ROOM_WIDTH / 2 - DOOR_WIDTH / 2, WALL_THICKNESS), "wall"));

                //Ensures player does not leave window boundaries, if he is unable to leave the room properly
                obstacles.Add(new Obstacle(new Rect(new Vector2((float)GRID_UNIT * 7, (float)GRID_UNIT * 9), GRID_UNIT, GRID_UNIT), "door"));
                obstacles.Add(new Obstacle(new Rect(new Vector2((float)GRID_UNIT * 8, (float)GRID_UNIT * 9), GRID_UNIT, GRID_UNIT), "door"));
            }
            else
            {
                //If an adjacent room in this direction does not exist, creates a solid wall
                walls.Add(new Obstacle(new Rect(new Vector2(0, ROOM_HEIGHT - WALL_THICKNESS), ROOM_WIDTH, WALL_THICKNESS), "wall"));
            }

            //Walls on the left side of the room
            if (connectedRooms[LEFT] != null)
            {
                //Creates opening to move to another room
                walls.Add(new Obstacle(new Rect(Vector2.Zero, WALL_THICKNESS, ROOM_HEIGHT / 2 - DOOR_WIDTH / 2), "wall"));
                walls.Add(new Obstacle(new Rect(new Vector2(0, ROOM_HEIGHT / 2 + DOOR_WIDTH / 2), WALL_THICKNESS, ROOM_HEIGHT / 2 - DOOR_WIDTH / 2), "wall"));

                //Ensures player does not leave window boundaries, if he is unable to leave the room properly
                obstacles.Add(new Obstacle(new Rect(new Vector2((float)GRID_UNIT * -1, (float)GRID_UNIT * 3), GRID_UNIT, GRID_UNIT), "door"));
                obstacles.Add(new Obstacle(new Rect(new Vector2((float)GRID_UNIT * -1, (float)GRID_UNIT * 4), GRID_UNIT, GRID_UNIT), "door"));
                obstacles.Add(new Obstacle(new Rect(new Vector2((float)GRID_UNIT * -1, (float)GRID_UNIT * 5), GRID_UNIT, GRID_UNIT), "door"));
            }
            else
            {
                //If an adjacent room in this direction does not exist, creates a solid wall
                walls.Add(new Obstacle(new Rect(Vector2.Zero, WALL_THICKNESS, ROOM_HEIGHT), "wall"));
            }

            //Creates hitboxes for doors, so the game can detect when a player is trying to move between rooms
            doorHitboxes[UP] = new Rect(new Vector2(ROOM_WIDTH / 2 - DOOR_WIDTH / 2, 0), DOOR_WIDTH, WALL_THICKNESS);
            doorHitboxes[RIGHT] = new Rect(new Vector2(ROOM_WIDTH - WALL_THICKNESS / 2, ROOM_HEIGHT / 2 - DOOR_WIDTH / 2), WALL_THICKNESS, DOOR_WIDTH);
            doorHitboxes[DOWN] = new Rect(new Vector2(ROOM_WIDTH / 2 - DOOR_WIDTH / 2, ROOM_HEIGHT - WALL_THICKNESS / 2), DOOR_WIDTH, WALL_THICKNESS);
            doorHitboxes[LEFT] = new Rect(new Vector2(0, ROOM_HEIGHT / 2 - DOOR_WIDTH / 2), WALL_THICKNESS, DOOR_WIDTH);

            foreach (Obstacle wall in walls)
            {
                obstacles.Add(wall);
            }

            //Updates enemy obsacle list
            foreach (Enemy e in enemies)
            {
                e.SetObstacles(walls);
            }
        }

        /// <summary>
        /// Updates all enemies in this Room
        /// </summary>
        /// <param name="projectileField">A valid ref List of Projectile objects</param>
        /// <param name="particleField">A valid ref List of Particles</param>
        /// <param name="playerLocation">A valid Vector2</param>
        /// <param name="stats">A valid PlayerStats</param>
        /// <param name="gameTime">A valid GameTime</param>
        public void Update(ref List<Projectile> projectileField, ref List<Particle> particleField, Vector2 playerLocation, ref PlayerStats stats, GameTime gameTime)
        {
            //Updates each enemy
            for(int i = 0; i < enemies.Count; i++)
            {
                //Updates the enemy
                enemies[i].Update(ref projectileField, playerLocation, gameTime);

                //Takes care of an enemy if its health is at 0
                if (enemies[i].IsDead())
                {
                    //The current enemy is the last enemy in the room
                    if (enemies.Count == 1)
                    {
                        //Gives the player 10-20 gold for clearing the room
                        stats.SetGold(stats.GetGold() + 15 + (new Random().Next(0, 10) - 5));

                        //Sound effect
                        roomClearEffect.Play();
                    }

                    //Summons a Sandpile in the place of a dead Sandman
                    if (enemies[i] is Sandman)
                    {
                        //Sets up a new Sandpile to replace the dead Sandman
                        Sandpile newSandpile = new Sandpile(content, enemies[i].GetLocation());
                        newSandpile.GetProjectileHitbox().SetCenter(enemies[i].GetProjectileHitbox().GetCenter());

                        //Adds the new Sandpile to the room
                        enemies.Add(newSandpile);
                        
                    }

                    //Splash effect
                    this.SplashEffect(ref particleField, enemies[i].GetProjectileHitbox().GetCenter(), Color.DarkRed);

                    //Wipes the dead enemy from memory
                    enemies.Remove(enemies[i]);
                    i--;

                    //Sound effect
                    enemyDeathEffect.Play();

                    //Does not bother checking the next condition, since the current enemy no longer exists
                    continue;
                }

                //Replaces any fully-healed Sandpiles with Sandmen
                if (enemies[i] is Sandpile && ((Sandpile)enemies[i]).IsFullyHealed())
                {
                    //Summons a Sandman in the place of the Sandpile
                    Sandman newSandman = new Sandman(content, enemies[i].GetLocation(), playerLocation, GRID_UNIT, ROOM_WIDTH, ROOM_HEIGHT, obstacles);
                    newSandman.GetProjectileHitbox().SetCenter(enemies[i].GetProjectileHitbox().GetCenter());
                    enemies.Add(newSandman);

                    //Splash effect
                    this.SplashEffect(ref particleField, enemies[i].GetProjectileHitbox().GetCenter(), Color.LightBlue);

                    //Wipes the Sandpile from memory
                    enemies.Remove(enemies[i]);
                    i--;

                    //Sound effect
                    sandpileRegenEffect.Play();
                }
            }
        }

        /// <summary>
        /// Adds an Item Pedestal to the room
        /// </summary>
        /// <param name="item">A valid PassiveEffect</param>
        public void AddItemPedestal(PassiveEffect item)
        {
            items.Add(new ItemPedestal(item));
        }

        /// <summary>
        /// The obstacles found in this Room
        /// </summary>
        /// <returns>A List of Obstacle objects</returns>
        public List<Obstacle> GetObstacles()
        {
            return obstacles;
        }

        /// <summary>
        /// The enemies found in this Room
        /// </summary>
        /// <returns>A list of Enemy objects</returns>
        public List<Enemy> GetEnemies()
        {
            return enemies;
        }

        /// <summary>
        /// The items found in this room
        /// </summary>
        /// <returns>A list of ItemPedestal objects</returns>
        public List<ItemPedestal> GetItemPedestals()
        {
            return items;
        }

        /// <summary>
        /// One of the 4 possible connected rooms
        /// </summary>
        /// <param name="index">The direction of the room, relative to this one</param>
        /// <returns>The specified room, or NULL if one does not exist in the specified direction</returns>
        public Room GetConnectedRoom(int index)
        {
            return connectedRooms[index];
        }

        /// <summary>
        /// Gets the door hitboxes
        /// </summary>
        /// <returns>An array storing Shapes, which represent this rooms doors' active hitboxes</returns>
        public Shape[] GetDoorHitboxes()
        {
            return doorHitboxes;
        }

        /// <summary>
        /// Indicates if the room has been entered before
        /// </summary>
        /// <returns>TRUE if this room has been entered before, othwerwise FALSE</returns>
        public bool HasBeenEntered()
        {
            return wasEntered;
        }

        /// <summary>
        /// Sets this rooms status as the given boolean
        /// </summary>
        /// <param name="entry">A valid bool</param>
        public void SetEntered(bool entry)
        {
            wasEntered = entry;
        }

        /// <summary>
        /// Sets a Room to be connected to this one
        /// </summary>
        /// <param name="direction">The direction that the new Room will be, relative to this one</param>
        /// <param name="r">A valid Room</param>
        public void SetConnectedRoom(int direction, Room r)
        {
            connectedRooms[direction] = r;
        }

        /// <summary>
        /// Creates a splash effect for an enemy death (It's not blood, so I can keep the ESRB "Everyone" rating)
        /// </summary>
        /// <param name="particleField">A valid ref List of Particles</param>
        /// <param name="location">Location to create particles around</param>
        /// <param name="colour">Colour of the particles</param>
        private void SplashEffect(ref List<Particle> particleField, Vector2 location, Color colour)
        {
            Random rng = new Random();

            for (int i = 0; i < 300; i++)
            {
                //Generates an angle for the projectile
                double partAngle = rng.Next(0, (int)(600 * Math.PI)) / 100;

                //Sets the particle's speed to a fraction of the projectile's speed
                double partSpeed = 7;

                //Sets the particle's rate of slowing or accelleration
                double partResistance = rng.Next(1, 121) / 100;

                //The particle will either fly straight, or turn clockwisecounterclockwise as it flies
                double partSpiralRate = rng.Next(-51, 51) / 100;

                //Adds the particle to the particleField
                particleField.Add(new Particle(location + new Vector2(rng.Next(-30, 30), rng.Next(-30, 30)), colour, partAngle, partSpeed, partResistance, partSpiralRate, 250));
            }
        }

        /// <summary>
        /// Generates a room with the specified layout
        /// </summary>
        /// <param name="roomID">Specifies layout</param>
        private void GenerateRoom(int roomID)
        {
            //Adding obstacles, based on given layout ID
            switch (roomID)
            {
                case 0:

                    //Reserved for starting room, no obstacles are present
                    
                    break;
                
                case 1:

                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 6), GRID_UNIT, GRID_UNIT), "rock"));

                    enemies.Add(new Nanocloud(content, this.GridPos(3, 2)));
                    enemies.Add(new Nanocloud(content, this.GridPos(12, 6)));

                    break;

                case 2:

                    obstacles.Add(new Obstacle(new Rect(this.GridPos(3, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(5, 2), GRID_UNIT, GRID_UNIT), "rock"));

                    obstacles.Add(new Obstacle(new Rect(this.GridPos(10, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(12, 3), GRID_UNIT, GRID_UNIT), "rock"));

                    obstacles.Add(new Obstacle(new Rect(this.GridPos(3, 5), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 5), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(5, 6), GRID_UNIT, GRID_UNIT), "rock"));

                    obstacles.Add(new Obstacle(new Rect(this.GridPos(10, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 5), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(12, 5), GRID_UNIT, GRID_UNIT), "rock"));

                    enemies.Add(new Nanocloud(content, this.GridPos(3, 2)));
                    enemies.Add(new Sandman(content, this.GridPos(12, 2), Vector2.Zero, GRID_UNIT, ROOM_WIDTH, ROOM_HEIGHT, obstacles));
                    enemies.Add(new Sandman(content, this.GridPos(3, 6), Vector2.Zero, GRID_UNIT, ROOM_WIDTH, ROOM_HEIGHT, obstacles));
                    enemies.Add(new Nanocloud(content, this.GridPos(12, 6)));

                    break;

                case 3:

                    obstacles.Add(new Obstacle(new Rect(this.GridPos(6, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(7, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(8, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(9, 3), GRID_UNIT, GRID_UNIT), "rock"));

                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(5, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(6, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(7, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(8, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(9, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(10, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(6, 5), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(7, 5), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(8, 5), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(9, 5), GRID_UNIT, GRID_UNIT), "rock"));

                    enemies.Add(new Turret(content, this.GridPos(3, 4)));
                    enemies.Add(new Turret(content, this.GridPos(12, 4)));

                    break;

                case 4:

                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(5, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(6, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(9, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(10, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 2), GRID_UNIT, GRID_UNIT), "rock"));

                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(5, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(6, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(9, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(10, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 5), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 5), GRID_UNIT, GRID_UNIT), "rock"));

                    enemies.Add(new WeepingTurret(content, this.GridPos(5, 3), Vector2.Zero, GRID_UNIT, ROOM_WIDTH, ROOM_HEIGHT, obstacles));
                    enemies.Add(new WeepingTurret(content, this.GridPos(10, 5), Vector2.Zero, GRID_UNIT, ROOM_WIDTH, ROOM_HEIGHT, obstacles));
                    enemies.Add(new BroodingTurret(content, this.GridPos(5, 5), Vector2.Zero, GRID_UNIT, ROOM_WIDTH, ROOM_HEIGHT, obstacles));
                    enemies.Add(new BroodingTurret(content, this.GridPos(10, 3), Vector2.Zero, GRID_UNIT, ROOM_WIDTH, ROOM_HEIGHT, obstacles));

                    break;

                case 5:
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(5, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(6, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(7, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(8, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(9, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(10, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(5, 5), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(6, 5), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(7, 5), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(8, 5), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(9, 5), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(10, 5), GRID_UNIT, GRID_UNIT), "rock"));

                    obstacles.Add(new Obstacle(new Rect(this.GridPos(5, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(10, 4), GRID_UNIT, GRID_UNIT), "rock"));

                    enemies.Add(new Kelad(content, this.GridPos(1, 1), Vector2.Zero, GRID_UNIT, ROOM_WIDTH, ROOM_HEIGHT, obstacles));
                    enemies.Add(new Kelad(content, this.GridPos(15, 8), Vector2.Zero, GRID_UNIT, ROOM_WIDTH, ROOM_HEIGHT, obstacles));

                    break;

                case 6:
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(6, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(9, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(3, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(12, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(6, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(9, 6), GRID_UNIT, GRID_UNIT), "rock"));

                    enemies.Add(new Sandpile(content, this.GridPos(4, 4)));
                    enemies.Add(new Sandpile(content, this.GridPos(11, 4)));

                    break;

                case 7:

                    obstacles.Add(new Obstacle(new Rect(this.GridPos(0, 0), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(1, 0), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(2, 0), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(3, 0), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 0), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(5, 0), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(0, 1), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(1, 1), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(2, 1), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(3, 1), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 1), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(5, 1), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(0, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(1, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(2, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(3, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(5, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(10, 0), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 0), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(12, 0), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(13, 0), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(14, 0), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(15, 0), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(10, 1), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 1), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(12, 1), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(13, 1), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(14, 1), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(15, 1), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(10, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(12, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(13, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(14, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(15, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(0, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(1, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(2, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(3, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(5, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(0, 7), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(1, 7), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(2, 7), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(3, 7), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 7), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(5, 7), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(0, 8), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(1, 8), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(2, 8), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(3, 8), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 8), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(5, 8), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(10, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(12, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(13, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(14, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(15, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(10, 7), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 7), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(12, 7), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(13, 7), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(14, 7), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(15, 7), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(10, 8), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 8), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(12, 8), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(13, 8), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(14, 8), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(15, 8), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    break;

                case 8:

                    obstacles.Add(new Obstacle(new Rect(this.GridPos(5, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(6, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(10, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(9, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(5, 5), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(6, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(10, 5), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(9, 6), GRID_UNIT, GRID_UNIT), "rock"));

                    enemies.Add(new Nanocloud(content, this.GridPos(8, 4)));

                    break;

                case 9:
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(6, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(7, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(8, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(9, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(6, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(7, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(8, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(9, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(3, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(3, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(3, 5), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(12, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(12, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(12, 5), GRID_UNIT, GRID_UNIT), "rock"));

                    enemies.Add(new WeepingTurret(content, this.GridPos(4, 4), Vector2.Zero, GRID_UNIT, ROOM_WIDTH, ROOM_HEIGHT, obstacles));
                    enemies.Add(new WeepingTurret(content, this.GridPos(11, 4), Vector2.Zero, GRID_UNIT, ROOM_WIDTH, ROOM_HEIGHT, obstacles));

                    break;

                case 10:
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(3, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(6, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(9, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(12, 3), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(2, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(3, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(6, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(7, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(8, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(9, 4), GRID_UNIT, GRID_UNIT), "rock"));

                    obstacles.Add(new Obstacle(new Rect(this.GridPos(12, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(13, 4), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(3, 5), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(6, 5), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(9, 5), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(12, 5), GRID_UNIT, GRID_UNIT), "rock"));

                    enemies.Add(new BroodingTurret(content, this.GridPos(4, 4), Vector2.Zero, GRID_UNIT, ROOM_WIDTH, ROOM_HEIGHT, obstacles));
                    enemies.Add(new BroodingTurret(content, this.GridPos(11, 4), Vector2.Zero, GRID_UNIT, ROOM_WIDTH, ROOM_HEIGHT, obstacles));

                    break;

                case 11:
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(0, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(1, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(2, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(3, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(5, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(10, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(12, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(13, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(14, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(15, 2), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(0, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(1, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(2, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(3, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(4, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(5, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(10, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(11, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(12, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(13, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(14, 6), GRID_UNIT, GRID_UNIT), "rock"));
                    obstacles.Add(new Obstacle(new Rect(this.GridPos(15, 6), GRID_UNIT, GRID_UNIT), "rock"));

                    enemies.Add(new Sandman(content, this.GridPos(1, 1), Vector2.Zero, GRID_UNIT, ROOM_WIDTH, ROOM_HEIGHT, obstacles));
                    enemies.Add(new Sandman(content, this.GridPos(15, 7), Vector2.Zero, GRID_UNIT, ROOM_WIDTH, ROOM_HEIGHT, obstacles));                    

                    break;
            }
        }

        /// <summary>
        /// A Vector2 holding an indicated position on the room grid
        /// </summary>
        /// <param name="x">A valid float</param>
        /// <param name="y">A valid float</param>
        /// <returns>A Vector2 object</returns>
        private Vector2 GridPos(float x, float y)
        {
            return new Vector2(GRID_UNIT * x, GRID_UNIT * y);
        }
    }
}
