//Author: Ilan Segal
//File Name: Obstacle.cs
//Project Name: RoboKnight
//Creation Date: December 9, 2015
//Modified Date: December 21, 2015
//Description: Class which represents an obstacle which the player or an enemy can collide with.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Roboknight
{
    class Obstacle
    {
        //The hitbox of this object
        private Shape hitbox;

        //The type of obstacle, used for drawing
        private string type;

        /// <summary>
        /// Constructor for the Obstacle class
        /// </summary>
        /// <param name="hitbox">The hitbox of this object</param>
        /// <param name="type">The type of Obstacle assigned to this object</param>
        public Obstacle(Rect hitbox, string type)
        {
            this.hitbox = hitbox;
            this.type = type;
        }

        /// <summary>
        /// Gets the hitbox
        /// </summary>
        /// <returns>This object's hitbox</returns>
        public Shape GetHitbox()
        {
            return hitbox;
        }

        /// <summary>
        /// Gets the type of Obstacle
        /// </summary>
        /// <returns>Returns this object's "type" string</returns>
        public string GetObstacleType()
        {
            return type;
        }
    }
}
