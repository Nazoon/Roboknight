//Author: Ilan Segal
//File Name: Shape.cs
//Project Name: RoboKnight
//Creation Date: December 5, 2015
//Modified Date: January 14, 2015
//Description: The class which represents the "Shape" object. Stores location and size.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class Shape
    {
        //Represents Shapes location in game
        protected Vector2 location;

        //Represents the Shapes radius (Circle) or length (Rectangle)
        protected double size;

        /// <summary>
        /// Constructor for the Shape class
        /// </summary>
        /// <param name="location">The location of the Shape</param>
        /// <param name="size">The size of the Shape</param>
        public Shape(Vector2 location, double size)
        {
            this.location = location;
            this.size = size;
        }

        /// <summary>
        /// Gethe top-left location of this Shape
        /// </summary>
        /// <returns>A Vector2 object</returns>
        public Vector2 GetLocation()
        {
            return location;
        }

        /// <summary>
        /// Gets the size of this Shape
        /// </summary>
        /// <returns>A double primitive</returns>
        public double GetSize()
        {
            return size;
        }

        /// <summary>
        /// Sets the top-right location of this Shape
        /// </summary>
        /// <param name="newLocation">A valid Vector2</param>
        public void SetLocation(Vector2 newLocation)
        {
            location = newLocation;
        }

        /// <summary>
        /// Sets the size for this Shape
        /// </summary>
        /// <param name="newSize">A valid double</param>
        public void SetSize(double newSize)
        {
            size = newSize;
        }

        /// <summary>
        /// Sets the center of this Shape (To be overidden by child classes)
        /// </summary>
        /// <param name="centerLocation">A valid Vector2</param>
        public virtual void SetCenter(Vector2 centerLocation)
        {
        }

        /// <summary>
        /// Gets the center for this Shape (To be overidden by child classes)
        /// </summary>
        /// <returns>A Vector2.Zero, unless overidden by child class</returns>
        public virtual Vector2 GetCenter()
        {
            return Vector2.Zero;
        }
    }
}
