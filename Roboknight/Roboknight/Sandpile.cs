//Author: Ilan Segal
//File Name: Sandpile.cs
//Project Name: RoboKnight
//Creation Date: December 26, 2015
//Modified Date: January 16, 2016
//Description: Class which represents the Sandpile enemy (subclass of Enemy). Appears when a Sandman is slain.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class Sandpile : Enemy
    {
        /// <summary>
        /// Constructor for the Sandpile slass
        /// </summary>
        /// <param name="content">A valid ContentLoader</param>
        /// <param name="location">Starting location</param>
        public Sandpile(ContentManager content, Vector2 location) : base(content, location)
        {
            //Setting up hitboxes
            projectileHitbox = new Circle(location, 70);
            obstacleHitbox = new Rect(location, 70, 70);

            //Stats
            health = 30;
            contactDamage = 0.5;
            dealsContactDamage = true;
        }

        /// <summary>
        /// Updates this Sandpile
        /// </summary>
        /// <param name="projectileField">A valid List of Projectiles</param>
        /// <param name="playerLocation">The player's location</param>
        /// <param name="gameTime">A valid GameTime</param>
        public override void Update(ref List<Projectile> projectileField, Vector2 playerLocation, GameTime gameTime)
        {
            //Regeneration
            health += 0.3;

            //Updating size to correspond with health
            projectileHitbox.SetSize(health / 150 * 70);
            ((Rect)obstacleHitbox).SetHeight(health / 150 * 70);
            ((Rect)obstacleHitbox).SetWidth(health / 150 * 70);
        }

        /// <summary>
        /// Indicates if the Sandpile is fully healed
        /// </summary>
        /// <returns>TRUE if this Sandpile is fully healed, otherwise FALSE</returns>
        public bool IsFullyHealed()
        {
            return (health >= 150);
        }
    }
}
