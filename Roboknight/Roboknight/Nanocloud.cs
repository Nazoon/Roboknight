//Author: Ilan Segal
//File Name: Nanocloud.cs
//Project Name: RoboKnight
//Creation Date: December 26, 2015
//Modified Date: January 16, 2016
//Description: Class which represents the Nanocloud enemy (subclass of Enemy)

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class Nanocloud : Enemy
    {
        /// <summary>
        /// Constructor for the Nanocloud slass
        /// </summary>
        /// <param name="content">A valid ContentLoader</param>
        /// <param name="location">Starting location</param>
        public Nanocloud(ContentManager content, Vector2 location) : base(content, location)
        {
            //Hitbox data
            projectileHitbox = new Circle(location, 150);
            obstacleHitbox = new Rect(location, 150, 150);

            //Stats
            health = 35;
            speed = 2.5;
            contactDamage = 0.5;
            dealsContactDamage = true;
        }

        /// <summary>
        /// Updates this Nanocloud
        /// </summary>
        /// <param name="projectileField">A valid List of Projectiles</param>
        /// <param name="playerLocation">The player's location</param>
        /// <param name="gameTime">A valid GameTime</param>
        public override void Update(ref List<Projectile> projectileField, Vector2 playerLocation, GameTime gameTime)
        {
            base.Update(ref projectileField, playerLocation, gameTime);

            //Calculates angle to move in
            double xDif = projectileHitbox.GetCenter().X - playerLocation.X;
            double yDif = projectileHitbox.GetCenter().Y - playerLocation.Y;
            double angle = Math.Atan(yDif / xDif);
            if (xDif > 0)
            {
                angle += Math.PI;
            }

            //Calculates movement vector
            Vector2 move = new Vector2((float)(speed * Math.Cos(angle)), (float)(speed * Math.Sin(angle)));

            //Applies movement vector
            location += move;
            projectileHitbox.SetLocation(projectileHitbox.GetLocation() + move);
            obstacleHitbox.SetLocation(obstacleHitbox.GetLocation() + move);
        }
    }
}
