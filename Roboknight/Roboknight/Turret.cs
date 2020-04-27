//Author: Ilan Segal
//File Name: Turret.cs
//Project Name: RoboKnight
//Creation Date: December 25, 2015
//Modified Date: January 16, 2016
//Description: Class which represents the Turret enemy (subclass of Enemy)

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class Turret : Enemy
    {
        /// <summary>
        /// Constructor for the Turret slass
        /// </summary>
        /// <param name="content">A valid ContentLoader</param>
        /// <param name="location">Starting location</param>
        public Turret(ContentManager content, Vector2 location): base(content, location)
        {
            //Setting up hitboxes
            projectileHitbox = new Circle(location, 70);
            obstacleHitbox = new Rect(location, 70, 70);

            //Stats
            health = 90;
            projectileDamage = 0.5;
            shotSpeed = 9;
            range = 800;
            fireRate = 800;
            timeSinceLastShot = 0;
            dealsContactDamage = false;
        }

        /// <summary>
        /// Updates this Turret
        /// </summary>
        /// <param name="projectileField">A valid List of Projectiles</param>
        /// <param name="playerLocation">The player's location</param>
        /// <param name="gameTime">A valid GameTime</param>
        public override void Update(ref List<Projectile> projectileField, Vector2 playerLocation, GameTime gameTime)
        {
            base.Update(ref projectileField, playerLocation, gameTime);

            //Updates timer
            timeSinceLastShot -= gameTime.ElapsedGameTime.Milliseconds;

            //Shoots if enough time has passed
            if (timeSinceLastShot <= 0)
            {
                this.FireProjectile(playerLocation, ref projectileField);
            }
        }
    }
}
