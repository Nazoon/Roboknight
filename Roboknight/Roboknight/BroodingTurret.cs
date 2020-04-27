//Author: Ilan Segal
//File Name: BroodingTurret.cs
//Project Name: RoboKnight
//Creation Date: December 26, 2015
//Modified Date: January 16, 2016
//Description: Class which represents the Brooding Turret enemy (subclass of Enemy)

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class BroodingTurret : Enemy
    {
        /// <summary>
        /// Constructor for the BroodingTurret class
        /// </summary>
        /// <param name="content">A valid ContentManager</param>
        /// <param name="location">The starting location of this enemy</param>
        /// <param name="playerLocation">The player's location</param>
        /// <param name="gridUnit">The size of a grid unit</param>
        /// <param name="roomWidth">Width of the room</param>
        /// <param name="roomHeight">Height of the room</param>
        /// <param name="roomObstacles">All obstacles in the room</param>
        public BroodingTurret(ContentManager content, Vector2 location, Vector2 playerLocation, int gridUnit, int roomWidth, int roomHeight, List<Obstacle> roomObstacles) : base(content, location)
        {
            //Creating pathfinding objects
            path = new Path(gridUnit, roomWidth, roomHeight, location, playerLocation, roomObstacles);
            cm = new CollisionManager();

            //Setting up hitboxes
            obstacleHitbox = new Rect(location, 75, 75);
            projectileHitbox = new Circle(location, 75);

            //Stats
            health = 150;
            speed = 6.5;
            projectileDamage = 0.5;
            contactDamage = 1;
            shotSpeed = 10;
            range = 200;
            fireRate = 700;
            timeSinceLastShot = 0;
            dealsContactDamage = true;

            //Pathfinding data
            timeUntilNextRefresh = 0;
            playerPrevLocation = Vector2.Zero;

            //Setting up obstacles
            roomObstacleList = new List<Shape>();
            foreach (Obstacle o in roomObstacles)
            {
                roomObstacleList.Add(o.GetHitbox());
            }
        }

        /// <summary>
        /// Updates this BroodingTurret
        /// </summary>
        /// <param name="projectileField">A valid List of Projectiles</param>
        /// <param name="playerLocation">The player's location</param>
        /// <param name="gameTime">A valid GameTime</param>
        public override void Update(ref List<Projectile> projectileField, Vector2 playerLocation, GameTime gameTime)
        {
            base.Update(ref projectileField, playerLocation, gameTime);

            //Updates timers
            timeSinceLastShot -= gameTime.ElapsedGameTime.Milliseconds;
            timeUntilNextRefresh -= gameTime.ElapsedGameTime.Milliseconds;

            //Deciding AI logic path
            double distToPlayer = Math.Sqrt((location.X - playerLocation.X) * (location.X - playerLocation.X) + (location.Y - playerLocation.Y) * (location.Y - playerLocation.Y));
            if (distToPlayer >= 200 && timeSinceLastShot <= 0)
            {
                //Firing projectile
                this.FireProjectile(playerLocation, ref projectileField);
            }
            else if (distToPlayer < 200)
            {
                //Refreshes path if necessary
                if ((timeUntilNextRefresh <= 0 && !playerLocation.Equals(playerPrevLocation)) || path.GetPath().Count == 0)
                {
                    this.RefreshPath(playerLocation);
                }

                //Follows player
                this.FollowPath();
            }
        }
    }
}
