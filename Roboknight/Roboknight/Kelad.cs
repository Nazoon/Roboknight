//Author: Ilan Segal
//File Name: Kelad.cs
//Project Name: RoboKnight
//Creation Date: December 26, 2015
//Modified Date: January 16, 2016
//Description: Class which represents the Kelad enemy (subclass of Enemy)

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class Kelad : Enemy
    {
        /// <summary>
        /// Constructor for the Kelad class
        /// </summary>
        /// <param name="content">A valid ContentManager</param>
        /// <param name="location">The starting location of this enemy</param>
        /// <param name="playerLocation">The player's location</param>
        /// <param name="gridUnit">The size of a grid unit</param>
        /// <param name="roomWidth">Width of the room</param>
        /// <param name="roomHeight">Height of the room</param>
        /// <param name="roomObstacles">All obstacles in the room</param>
        public Kelad(ContentManager content, Vector2 location, Vector2 playerLocation, int gridUnit, int roomWidth, int roomHeight, List<Obstacle> roomObstacles) : base(content, location)
        {
            //Creating pathfinding objects
            path = new Path(gridUnit, roomWidth, roomHeight, location, playerLocation, roomObstacles);
            cm = new CollisionManager();

            //Setting up hitboxes
            obstacleHitbox = new Rect(location, 40, 40);
            projectileHitbox = new Circle(location, 40);

            //Stats
            health = 180;
            speed = 1;
            projectileDamage = 1;
            shotSpeed = 10;
            range = 200;
            fireRate = 200;
            timeSinceLastShot = 0;
            dealsContactDamage = false;

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
        /// Updates this Kelad
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

            //Shoots projectile if enough time has passed since last shot
            if (timeSinceLastShot <= 0)
            {
                this.FireProjectile(playerLocation, ref projectileField);
            }

            //Refreshes path if necessary
            if ((timeUntilNextRefresh <= 0 && !playerLocation.Equals(playerPrevLocation)) || path.GetPath().Count == 0)
            {
                this.RefreshPath(playerLocation);
            }

            //Follows path
            this.FollowPath();
        }
    }
}
