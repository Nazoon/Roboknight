//Author: Ilan Segal
//File Name: Square.cs
//Project Name: RoboKnight
//Creation Date: December 5, 2015
//Modified Date: January 14, 2016
//Description: The class which represents the "Rect" object. Child of "Shape" class.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class Rect : Shape
    {
        private double sideWidth;

        /// <summary>
        /// Constructor for the Square class
        /// </summary>
        /// <param name="location">Location of the top-left corner of the Rect</param>
        /// <param name="sideHeight">Side height of the Rect</param>
        /// <param name="sideWidth">Side width of the Rect</param>
        public Rect(Vector2 location, double sideWidth, double sideHeight) : base(location, sideHeight)
        {
            this.sideWidth = sideWidth;
        }

        /// <summary>
        /// Gets the width of this Rect
        /// </summary>
        /// <returns>A double primitive</returns>
        public double GetWidth()
        {
            return sideWidth;
        }

        /// <summary>
        /// Gets the height of this Rect
        /// </summary>
        /// <returns>A double primitive</returns>
        public double GetHeight()
        {
            return size;
        }

        /// <summary>
        /// Sets the width of this Rect
        /// </summary>
        /// <param name="newWidth">A valid double</param>
        public void SetWidth(double newWidth)
        {
            sideWidth = newWidth;
        }
        
        /// <summary>
        /// Sets the height of this Rect
        /// </summary>
        /// <param name="newHeight">A valid double</param>
        public void SetHeight(double newHeight)
        {
            size = newHeight;
        }

        /// <summary>
        /// Gets the center of this Rect
        /// </summary>
        /// <returns>A Vector2, indicating the center of this Rect shape</returns>
        public override Vector2 GetCenter()
        {
            return new Vector2((float)(location.X + sideWidth / 2), (float)(location.Y + size / 2));
        }

        /// <summary>
        /// Sets the center of this Rect
        /// </summary>
        /// <param name="centerLocation">A valid Vector2</param>
        public override void SetCenter(Vector2 centerLocation)
        {
            //Sets new location
            location.X = (float)(centerLocation.X - sideWidth / 2);
            location.Y = (float)(centerLocation.Y - size / 2);
        }
    }
}
