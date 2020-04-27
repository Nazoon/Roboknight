//Author: Ilan Segal
//File Name: Circle.cs
//Project Name: RoboKnight
//Creation Date: December 5, 2015
//Modified Date: January 14, 2016
//Description: The class which represents the "Cirlce" object. Child of "Shape" class.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class Circle : Shape
    {
        /// <summary>
        /// Constructor for the Circle class
        /// </summary>
        /// <param name="location">Location of the centre of the Cirlce</param>
        /// <param name="radius">Radius of the Circle</param>
        public Circle(Vector2 location, double radius) : base (location, radius)
        {
        }

        /// <summary>
        /// Gets the circle's radius
        /// </summary>
        /// <returns>Half of the Shapes 'Size' value</returns>
        public double GetRadius()
        {
            return size / 2;
        }

        /// <summary>
        /// Gets the position of the center of the circle
        /// </summary>
        /// <returns>A Vector2 object</returns>
        public override Vector2 GetCenter()
        {
            float xPos = (float)(this.location.X + (this.size / 2));
            float yPos = (float)(this.location.Y + (this.size / 2));

            return new Vector2(xPos, yPos);
        }

        /// <summary>
        /// Sets the position of the center of the circle
        /// </summary>
        /// <param name="centerLocation">A valid Vector2 object</param>
        public override void SetCenter(Vector2 centerLocation)
        {
            float xPos = (float)(centerLocation.X - (this.size / 2));
            float yPos = (float)(centerLocation.Y - (this.size / 2));

            this.location = new Vector2(xPos, yPos);
        }
    }
}
