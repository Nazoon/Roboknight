//Author: Ilan Segal
//File Name: Sandman.cs
//Project Name: RoboKnight
//Creation Date: December 26, 2015
//Modified Date: January 16, 2016
//Description: Class which represents the Sandman enemy (subclass of Enemy). Appears when a Sandpile is allowed to fully heal.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class Sandman : Enemy
    {
        /// <summary>
        /// Constructor for the Sandman class
        /// </summary>
        /// <param name="content">A valid ContentManager</param>
        /// <param name="location">The starting location of this enemy</param>
        /// <param name="playerLocation">The player's location</param>
        /// <param name="gridUnit">The size of a grid unit</param>
        /// <param name="roomWidth">Width of the room</param>
        /// <param name="roomHeight">Height of the room</param>
        /// <param name="roomObstacles">All obstacles in the room</param>
        public Sandman(ContentManager content, Vector2 location, Vector2 playerLocation, int gridUnit, int roomWidth, int roomHeight, List<Obstacle> roomObstacles) : base(content, location)
        {
            //Creating pathfinding objects
            path = new Path(gridUnit, roomWidth, roomHeight, location, playerLocation, roomObstacles);
            cm = new CollisionManager();

            //Setting up hitboxes
            obstacleHitbox = new Rect(location, 70, 70);
            projectileHitbox = new Circle(location, 70);

            //Stats
            health = 150;
            speed = 4.2;
            contactDamage = 1.5;
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
        /// Updates this Kelad
        /// </summary>
        /// <param name="projectileField">A valid List of Projectiles</param>
        /// <param name="playerLocation">The player's location</param>
        /// <param name="gameTime">A valid GameTime</param>
        public override void Update(ref List<Projectile> projectileField, Vector2 playerLocation, GameTime gameTime)
        {
            base.Update(ref projectileField, playerLocation, gameTime);

            //Updates timers
            timeUntilNextRefresh -= gameTime.ElapsedGameTime.Milliseconds;
            
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
