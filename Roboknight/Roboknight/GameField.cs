//Author: Ilan Segal
//File Name: GameField.cs
//Project Name: RoboKnight
//Creation Date: December 14, 2015
//Modified Date: January 18, 2016
//Description: Hosts all the basic mechanics of the game, and facilitates interactions between the game classes.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Roboknight
{
    class GameField
    {
        //Game objects
        private Floor gameFloor;
        private PlayerCharacter player;

        //Grace period data (for when the player gets hurt)
        private bool gracePeriod;
        private int gracePeriodTimer;
        private const int MAX_GRACE_PERIOD_TIME = 3000;

        //Used to detect player or enemy collisions with obstacles
        private CollisionManager cm;

        //Used to manage player's location when moving between rooms
        private Vector2[] enterRoomLocations;

        //Projectiles data
        private List<Projectile> projectileField;

        //Particles on screen
        private List<Particle> particleField;
        private const int MAX_PARTICLES = 1000;

        //Sound effects
        private SoundEffect playerHitEffect;
        private SoundEffect playerDeathEffect;
        private SoundEffect enemyHitEffect;
        private SoundEffect projectileLandEffect;
        private SoundEffect roomEnterEffect;
        private SoundEffect itemRoomEnterEffect;
        private SoundEffect itemPickupEffect;

        //Dealing with wall-relative positions
        private const int ROOM_HEIGHT = 675;
        private const int ROOM_WIDTH = 1200;

        //Used when managing rooms' positions relative to eachother
        private const int UP = 0;
        private const int RIGHT = 1;
        private const int DOWN = 2;
        private const int LEFT = 3;

        //Used for particle spread when a projectile collides with an obstacle or player
        private const double PARTICLE_SPREAD_MAX = Math.PI * 0.167;
        private const double PARTICLE_SPREAD_MIN = Math.PI * -0.167;

        /// <summary>
        /// Constructor for the GameArea class
        /// </summary>
        /// <param name="content">A valid ContentManager</param>
        /// <param name="floorSize">The bounds placed on how big a floor can get</param>
        public GameField(ContentManager content, int floorSize)
        {
            //The floor is generated
            gameFloor = new Floor(content, floorSize);

            //The first room starts as being visited
            gameFloor.GetCurrentRoom().SetEntered(true);

            //The player starts at the centre of the room
            player = new PlayerCharacter(content, Vector2.Zero);
            player.SetLocation(new Vector2((float)(ROOM_WIDTH / 2 - player.GetObstacleHitbox().GetSize() / 2), (float)(ROOM_HEIGHT / 2 - player.GetObstacleHitbox().GetSize() / 2)));

            //Setting up grace period data
            gracePeriod = false;
            gracePeriodTimer = 0;

            //Used for detecting various collisions within the game
            cm = new CollisionManager();
            
            //Setting up room entry starting locations
            float playerHeight = (float)((Rect)player.GetObstacleHitbox()).GetHeight();
            float playerWidth = (float)((Rect)player.GetObstacleHitbox()).GetWidth();

            //Setting up initial player locations for after entering rooms
            enterRoomLocations = new Vector2[4];
            enterRoomLocations[UP] = new Vector2(ROOM_WIDTH / 2 - playerWidth / 2, 50);
            enterRoomLocations[RIGHT] = new Vector2(ROOM_WIDTH - 50 - playerWidth, ROOM_HEIGHT / 2 - playerHeight / 2);
            enterRoomLocations[DOWN] = new Vector2(ROOM_WIDTH / 2 - playerWidth / 2, ROOM_HEIGHT - 50 - playerHeight);
            enterRoomLocations[LEFT] = new Vector2(50, ROOM_HEIGHT / 2 - playerHeight / 2);

            //The game starts with no projectiles or particles on screen
            projectileField = new List<Projectile>();
            particleField = new List<Particle>();

            //Loading sound effects
            playerHitEffect = content.Load<SoundEffect>(@"Sound\Player\hit");
            playerDeathEffect = content.Load<SoundEffect>(@"Sound\Player\dead");
            enemyHitEffect = content.Load<SoundEffect>(@"Sound\Enemy\hit");
            projectileLandEffect = content.Load<SoundEffect>(@"Sound\Misc\land");
            roomEnterEffect = content.Load<SoundEffect>(@"Sound\Misc\roomenter");
            itemRoomEnterEffect = content.Load<SoundEffect>(@"Sound\Misc\itemroomentry");
            itemPickupEffect = content.Load<SoundEffect>(@"Sound\Menu\sellitem");
        }

        /// <summary>
        /// Updates the GameField
        /// </summary>
        /// <param name="gameTime">A valid Gametime</param>
        /// <param name="moveVector">A valid Vector2</param>
        /// <param name="weaponDirection">A valid WeaponState</param>
        /// <param name="speedPercent">A valid float, from 0 to 100</param>
        public void Update(GameTime gameTime, Vector2 moveVector, WeaponState weaponDirection)
        {
            //Moves the character
            this.MoveCharacter(gameFloor.GetCurrentRoom().GetObstacles(), moveVector);

            //Checks if the player is firing a projectile
            this.player.FireProjectile(ref projectileField, weaponDirection, gameTime);

            //Updates projectile field
            this.UpdateProjectiles();

            //Checks for contact damage to the player
            this.UpdateContactDamage();

            //Updates invincibility
            this.UpdateGracePeriod(gameTime);

            //Updates the current room's data
            PlayerStats playerStats = player.GetStats();
            gameFloor.GetCurrentRoom().Update(ref projectileField, ref particleField, player.GetProjectileHitbox().GetCenter(), ref playerStats, gameTime);

            //Allows player to pick up any items they are standing over
            this.UpdateItemPedestals();

            //Updates all particles on screen
            this.UpdateParticles(gameTime);

            //Checks if the player will move between rooms
            if (gameFloor.GetCurrentRoom().GetEnemies().Count == 0)
            {
                this.UpdateRoomLocation();
            }
        }

        /// <summary>
        /// The PlayerCharacter contained in this GameField
        /// </summary>
        /// <returns>The stored PlayerCharacter</returns>
        public PlayerCharacter GetPlayer()
        {
            return player;
        }

        /// <summary>
        /// Indicates if the player is currently invincible
        /// </summary>
        /// <returns>TRUE if the player is invincible, otherwise FALSE</returns>
        public bool IsPlayerInGracePeriod()
        {
            return gracePeriod;
        }

        /// <summary>
        /// Gets the invincibility timer
        /// </summary>
        /// <returns>The amount of time that the player has been in grade period, in milliseconds</returns>
        public int GetInvincibilityTimer()
        {
            return gracePeriodTimer;
        }

        /// <summary>
        /// The room that the Player is currently in
        /// </summary>
        /// <returns>The current room</returns>
        public Room GetCurrentRoom()
        {
            return gameFloor.GetCurrentRoom();
        }

        /// <summary>
        /// Gets the projectiles currently on screen
        /// </summary>e\
        /// <returns>A List of Projectiles</returns>
        public List<Projectile> GetProjectiles()
        {
            return projectileField;
        }

        /// <summary>
        /// Gets the particles currently on screen
        /// </summary>
        /// <returns>A list of Particles</returns>
        public List<Particle> GetParticles()
        {
            return particleField;
        }

        /// <summary>
        /// Gets the enemies currently in the room
        /// </summary>
        /// <returns>A List of Enemy objects</returns>
        public List<Enemy> GetEnemies()
        {
            return gameFloor.GetCurrentRoom().GetEnemies();
        }

        /// <summary>
        /// Gets the item pedestals found in the current room
        /// </summary>
        /// <returns>A List of ItemPedestal objects</returns>
        public List<ItemPedestal> GetItemPedestals()
        {
            return gameFloor.GetCurrentRoom().GetItemPedestals();
        }

        /// <summary>
        /// Moves the character in a certain direction
        /// </summary>
        /// <param name="obstacleList">Obstacles that the player could collide with</param>
        /// <param name="moveVector">A valid Vector2</param>
        /// <param name="speedPercent">A valid float, from 0 to 1</param>
        private void MoveCharacter(List<Obstacle> obstacleList, Vector2 moveVector)
        {
            //Gets the hitboxes of all obstacles in obstacleList
            List<Shape> roomObstacles = new List<Shape>();
            for (int i = 0; i < obstacleList.Count; i++)
            {
                roomObstacles.Add(obstacleList[i].GetHitbox());
            }

            //Attempts to move the player left/right without hitting any obstacles
            if (cm.WillCollide(player.GetObstacleHitbox(), roomObstacles, new Vector2((float)(moveVector.X * player.GetStats().GetSpeed()), 0)) == false)
            {
                player.Move((float)(moveVector.X * player.GetStats().GetSpeed()), 0);
            }

            //Attempts to move the player up/down without hitting obstacles
            if (cm.WillCollide(player.GetObstacleHitbox(), roomObstacles, new Vector2(0, (float)(moveVector.Y * player.GetStats().GetSpeed()))) == false)
            {
                player.Move(0, (float)(moveVector.Y * player.GetStats().GetSpeed()));
            }
        }

        /// <summary>
        /// Updates the projectiles currently on screen
        /// </summary>
        private void UpdateProjectiles()
        {
            //Loops through each projectile in projectileField
            for (int i = 0; i < projectileField.Count; i++)
            {
                //Updates its location
                projectileField[i].Update(ref particleField);
                                
                //If a projectile has expended its range, or has collided with an obstacle, remove it from projectileField
                if (projectileField[i].IsOutOfRange() || cm.WillCollide(projectileField[i].GetHitbox(), this.GetAllObstacleShapes(), Vector2.Zero) == true)
                {
                    if (projectileField[i].IsOutOfRange())
                    {
                        //Splash effect
                        this.SplashEffect(projectileField[i]);
                    }
                    else
                    {
                        //Impact effect
                        this.ImpactSpreadEffect(projectileField[i]);
                    }

                    //Removes projectile from projectileField
                    projectileField.RemoveAt(i);
                    i--;

                    //Sound effect
                    projectileLandEffect.Play(0.5F, -1F, 0F);
                }
                //If a *player* projectile has collided with an enemy, remove it from projectileField and deal damage to the enemy
                else if (projectileField[i].GetProjectileType().Equals("player"))
                {
                    //Homes projectiles to nearest enemy, if the player has the ability and there is an enemy to home in on
                    if (player.GetStats().HasHomingShot() && gameFloor.GetCurrentRoom().GetEnemies().Count > 0)
                    {
                        //Calculating target angle to nearest enemy
                        double xDif = projectileField[i].GetHurtbox().GetCenter().X - this.GetClosestEnemy(projectileField[i].GetHurtbox().GetCenter()).GetProjectileHitbox().GetCenter().X;
                        double yDif = projectileField[i].GetHurtbox().GetCenter().Y - this.GetClosestEnemy(projectileField[i].GetHurtbox().GetCenter()).GetProjectileHitbox().GetCenter().Y;
                        double targetAngle = Math.Atan(yDif / xDif);
                        if (xDif > 0)
                        {
                            targetAngle += Math.PI;
                        }

                        projectileField[i].SetAngle(projectileField[i].GetAngle() * 0.9 + targetAngle * 0.1);
                    }

                    foreach (Enemy e in gameFloor.GetCurrentRoom().GetEnemies())
                    {
                        //Checks if the hitboxes are overlapping
                        if (cm.HasCollided((Circle)projectileField[i].GetHurtbox(), (Circle)e.GetProjectileHitbox()) == true)
                        {
                            //The enemy takes damage
                            e.TakeDamage(projectileField[i].GetDamage());

                            //Splash effect
                            this.ImpactSpreadEffect(projectileField[i]);

                            //The projectile has collided, and so it is destroyed
                            projectileField.RemoveAt(i);
                            i--;

                            //Sound effect
                            enemyHitEffect.Play();

                            //Stop checking enemies
                            break;
                        }
                    }
                }
                //If an *enemy* projectile has collided with a player, remove it from projectileField and deal damage to the player
                else if (projectileField[i].GetProjectileType().Equals("enemy"))
                {
                    //Checks if the hitboxes are overlapping
                    if (cm.HasCollided((Circle)projectileField[i].GetHurtbox(), (Circle)player.GetProjectileHitbox()) == true)
                    {
                        //If the player is not in a grade period
                        if (gracePeriod == false)
                        {
                            //The player takes damage
                            player.TakeDamage(projectileField[i].GetDamage());
                            gracePeriod = true;

                            //Sound effect
                            playerHitEffect.Play();
                        }

                        //Splash effect
                        this.ImpactSpreadEffect(projectileField[i]);

                        //The projectile has collided, and so it is destroyed
                        projectileField.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// Checks for contact damage in the current update
        /// </summary>
        private void UpdateContactDamage()
        {
            //Loops through each enemy as long as its possible for the player to take damage
            for (int i = 0; i < gameFloor.GetCurrentRoom().GetEnemies().Count && gracePeriod == false; i++)
            {
                //Checks for overlapping hitboxes
                if (gameFloor.GetCurrentRoom().GetEnemies()[i].DealsContactDamage() == true && 
                    cm.HasCollided((Rect)player.GetObstacleHitbox(), (Rect)gameFloor.GetCurrentRoom().GetEnemies()[i].GetObstacleHitbox()) == true)
                {
                    //Sound effect
                    playerHitEffect.Play();

                    //Player takes damage
                    player.TakeDamage(gameFloor.GetCurrentRoom().GetEnemies()[i].GetContactDamage());
                    gracePeriod = true;
                }
            }
        }

        /// <summary>
        /// Moves player between rooms if they are moving through a door
        /// </summary>
        private void UpdateRoomLocation()
        {
            //Checks if the player will move to the next room up
            if (cm.HasCollided((Rect)(player.GetObstacleHitbox()), (Rect)(gameFloor.GetCurrentRoom().GetDoorHitboxes()[UP])) == true)
            {
                //Sets the current room to the next room up
                gameFloor.SetCurrentRoom(gameFloor.GetCurrentRoom().GetConnectedRoom(UP));

                //Sets the player's new location
                player.SetLocation(enterRoomLocations[DOWN]);

                //Clears projectiles and particles from the screen
                projectileField.Clear();
                particleField.Clear();

                //Sound effect
                if (gameFloor.GetCurrentRoom().GetItemPedestals().Count > 0 && gameFloor.GetCurrentRoom().HasBeenEntered() == false)
                {
                    //Player has found a new item room
                    //Special sound effect
                    itemRoomEnterEffect.Play(0.3F, 0F, 0F);
                }
                else
                {
                    //Ordinary room, or the item room has already been entered
                    //Normal sound effect
                    roomEnterEffect.Play(0.5F, -0.5F, 0F);
                }

                //The newly entered room is marked as entered
                gameFloor.GetCurrentRoom().SetEntered(true);
            }
            //Checks if the player will move to the next room to the right
            else if (cm.HasCollided((Rect)(player.GetObstacleHitbox()), (Rect)(gameFloor.GetCurrentRoom().GetDoorHitboxes()[RIGHT])) == true)
            {
                //Sets the current room to the next room to the right
                gameFloor.SetCurrentRoom(gameFloor.GetCurrentRoom().GetConnectedRoom(RIGHT));

                //Sets the player's new location
                player.SetLocation(enterRoomLocations[LEFT]);

                //Clears projectiles and particles from the screen
                projectileField.Clear();
                particleField.Clear();

                //The newly entered room is marked as entered
                gameFloor.GetCurrentRoom().SetEntered(true);

                //Sound effect
                roomEnterEffect.Play(0.5F, -0.5F, 0F);
            }
            //Checks if the player will move to the next room down
            else if (cm.HasCollided((Rect)(player.GetObstacleHitbox()), (Rect)(gameFloor.GetCurrentRoom().GetDoorHitboxes()[DOWN])) == true)
            {
                //Sets the current room to the next room down
                gameFloor.SetCurrentRoom(gameFloor.GetCurrentRoom().GetConnectedRoom(DOWN));

                //Sets the player's new location
                player.SetLocation(enterRoomLocations[UP]);

                //Clears projectiles and particles from the screen
                projectileField.Clear();
                particleField.Clear();

                //The newly entered room is marked as entered
                gameFloor.GetCurrentRoom().SetEntered(true);

                //Sound effect
                roomEnterEffect.Play(0.5F, -0.5F, 0F);
            }
            //Checks if the player will move to the next room to the left
            else if (cm.HasCollided((Rect)(player.GetObstacleHitbox()), (Rect)(gameFloor.GetCurrentRoom().GetDoorHitboxes()[LEFT])) == true)
            {
                //Sets the current room to the next room to the left
                gameFloor.SetCurrentRoom(gameFloor.GetCurrentRoom().GetConnectedRoom(LEFT));

                //Sets the player's new location
                player.SetLocation(enterRoomLocations[RIGHT]);

                //Clears projectiles and particles from the screen
                projectileField.Clear();
                particleField.Clear();

                //The newly entered room is marked as entered
                gameFloor.GetCurrentRoom().SetEntered(true);

                //Sound effect
                roomEnterEffect.Play(0.5F, -0.5F, 0F);
            }
        }

        /// <summary>
        /// Updates item pedestals
        /// </summary>
        private void UpdateItemPedestals()
        {
            foreach(ItemPedestal itemPedestal in gameFloor.GetCurrentRoom().GetItemPedestals())
            {
                //Only adds effect if the item pedestal contains an item, and the player is standing over the pedestal
                if (itemPedestal.GetItem() != PassiveEffect.None && cm.HasCollided((Circle)player.GetProjectileHitbox(), (Circle)itemPedestal.GetHitbox()) == true)
                {
                    //Adds effect to player
                    player.AddPassiveEffect(itemPedestal.GetItem());

                    //Removes item from pedestal
                    itemPedestal.SetItem(PassiveEffect.None);

                    //Sound effect
                    itemPickupEffect.Play();
                }
            }
        }

        /// <summary>
        /// Updates the particleField
        /// </summary>
        private void UpdateParticles(GameTime gameTime)
        {
            //Updates each particle in particleField
            for (int i = 0; i < particleField.Count; i++)
            {
                //Updates the current particle
                particleField[i].Update(gameTime);

                //Removes the current particle from the list if it has expended its lifespan
                if (particleField[i].IsOutOfRange() == true)
                {
                    particleField.RemoveAt(i);
                    i--;
                }
            }

            //Ensures there are not too many particles on screen
            while (particleField.Count > MAX_PARTICLES)
            {
                //Removes the oldest particles
                particleField.RemoveAt(0);
            }
        }

        /// <summary>
        /// Collects all obstacle hitboxes in the current room
        /// </summary>
        /// <returns>A list of Shapes</returns>
        private List<Shape> GetAllObstacleShapes()
        {
            //Creates the list
            List<Shape> returnList = new List<Shape>();

            //Collects all obstacle hitboxes
            foreach (Obstacle obstacle in gameFloor.GetCurrentRoom().GetObstacles())
            {
                returnList.Add(obstacle.GetHitbox());
            }

            //Returns the collection
            return returnList;
        }

        /// <summary>
        /// Updates the player's invincibility
        /// </summary>
        /// <param name="gameTime">A valid GameTime</param>
        private void UpdateGracePeriod(GameTime gameTime)
        {
            //Only operates if the player is in a grace period
            if (gracePeriod == true)
            {
                //Updates the timer
                gracePeriodTimer += gameTime.ElapsedGameTime.Milliseconds;

                //If grace period has run out, reset invincibility
                if (gracePeriodTimer >= MAX_GRACE_PERIOD_TIME)
                {
                    gracePeriodTimer = 0;
                    gracePeriod = false;
                }
            }
        }

        /// <summary>
        /// Creates a splash effect
        /// </summary>
        /// <param name="projectile">A valid Projectile</param>
        private void SplashEffect(Projectile projectile)
        {
            //Used to create random attributes
            Random rng = new Random();

            for (int i = 0; i < 200; i++)
            {
                //Generates an angle for the projectile
                double partAngle =  rng.Next(0, (int)(600 * Math.PI)) / 100;

                //Sets the particle's speed to a fraction of the projectile's speed
                double partSpeed = projectile.GetSpeed() / 5;

                //Sets the particle's rate of slowing or accelleration
                double partResistance = rng.Next(1, 121) / 100;

                //The particle will either fly straight, or turn clockwisecounterclockwise as it flies
                double partSpiralRate = rng.Next(-51, 51) / 100;

                //Adds the particle to the particleField
                particleField.Add(new Particle(projectile.GetHurtbox().GetCenter() + new Vector2(rng.Next(-30, 30), rng.Next(-30, 30)), projectile.GetColour(), partAngle, partSpeed, partResistance, partSpiralRate, 250));
            }
        }

        /// <summary>
        /// Creates an impact effect
        /// </summary>
        /// <param name="projectile">A valid Projectile</param>
        private void ImpactSpreadEffect(Projectile projectile)
        {
            Random rng = new Random();

            for (int i = 0; i < 100; i++)
            {
                //Generates an angle for the projectile
                double partAngle = ((double)(rng.Next((int)(PARTICLE_SPREAD_MIN * 1000), (int)(PARTICLE_SPREAD_MAX * 1000)))) / 1000 + projectile.GetAngle() + Math.PI;

                //Sets the particle's speed to a fraction of the projectile's speed
                double partSpeed = projectile.GetSpeed() / 3 + rng.Next(0, 5);

                //Sets the particle's rate of slowing or accelleration
                double partResistance = rng.Next(1, 121) / 100;

                //The particle will either fly straight, or turn clockwisecounterclockwise as it flies
                double partSpiralRate = rng.Next(-51, 51) / 100;

                //Adds the particle to the particleField
                particleField.Add(new Particle(projectile.GetHurtbox().GetCenter() + new Vector2(rng.Next(-30, 30), rng.Next(-30, 30)), projectile.GetColour(), partAngle, partSpeed, partResistance, partSpiralRate, 250));
            }
        }

        /// <summary>
        /// Gets the enemy closest to a given location
        /// </summary>
        /// <param name="location">A valid Vector2</param>
        /// <returns>The Enemy in the current room closest to the given Vector2</returns>
        private Enemy GetClosestEnemy(Vector2 location)
        {
            if (gameFloor.GetCurrentRoom().GetEnemies().Count > 0)
            {
                Enemy closest = gameFloor.GetCurrentRoom().GetEnemies()[0];

                foreach (Enemy enemy in gameFloor.GetCurrentRoom().GetEnemies())
                {
                    if (this.DiagonalLength(location, enemy.GetProjectileHitbox().GetCenter()) < this.DiagonalLength(location, closest.GetProjectileHitbox().GetCenter()))
                    {
                        closest = enemy;
                    }
                }

                return closest;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the diagonal length between two points
        /// </summary>
        /// <param name="v1">A valid Vector2</param>
        /// <param name="v2">A valid Vector2</param>
        /// <returns>Diagonal length between the given Vector2 objects</returns>
        private double DiagonalLength(Vector2 v1, Vector2 v2)
        {
            //Calculates values in difference
            double xDif = v1.X - v2.X;
            double yDif = v1.Y - v2.Y;

            //Returns diagonal length
            return Math.Sqrt(xDif * xDif + yDif * yDif);
        }
    }
}
