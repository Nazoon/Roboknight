//Author: Ilan Segal
//File Name: CollisonManager.cs
//Project Name: RoboKnight
//Creation Date: December 5, 2015
//Modified Date: January 8, 2016
//Description: The class which represents the "CollisionManager" object. Used to detect collisions between Shapes.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class CollisionManager
    {
        /// <summary>
        /// Constructor for the CollisionManager class
        /// </summary>
        public CollisionManager()
        {
        }

        /// <summary>
        /// Checks for collisions between Circles
        /// </summary>
        /// <param name="c1">A valid Circle</param>
        /// <param name="c2">A valid Circle</param>
        /// <returns>TRUE only if the Circles have collided</returns>
        public bool HasCollided(Circle c1, Circle c2)
        {
            //Calculates difference of X and Y axis-position of the two Circles
            double xDif = c1.GetCenter().X - c2.GetCenter().X;
            double yDif = c1.GetCenter().Y - c2.GetCenter().Y;

            //Calculates diagonal distance between the centres of the given Circles
            double distance = Math.Sqrt(xDif * xDif + yDif * yDif);

            //Calculates minimum diagonal distance required for collision (sum of the radii of the two circles)
            double minDistance = c1.GetRadius() + c2.GetRadius();

            //Returns TRUE only if the distance meets or exceeds the minimum distance
            return (distance <= minDistance);
        }

        /// <summary>
        /// Checks for collisions between Rects
        /// </summary>
        /// <param name="s1">A valid Rect</param>
        /// <param name="s2">A valid Rect</param>
        /// <returns>TRUE only if the Rect have collided</returns>
        public bool HasCollided(Rect s1, Rect s2)
        {
            //Returns true only if no gap exists between the Squares (Checks all four sides)
            return (s1.GetLocation().X - 1 < s2.GetLocation().X + s2.GetWidth() &&
                    s2.GetLocation().X - 1 < s1.GetLocation().X + s1.GetWidth() &&
                    s1.GetLocation().Y - 1 < s2.GetLocation().Y + s2.GetHeight() &&
                    s2.GetLocation().Y - 1 < s1.GetLocation().Y + s1.GetHeight());
        }

        /// <summary>
        /// Detects a predicted collision between two shapes, given a direction. (Given Shapes must be of the same sub-class)
        /// </summary>
        /// <param name="s1">A valid Shape</param>
        /// <param name="s2">A valid Shape</param>
        /// <param name="move">The direction in which the Shape "s1" will try to move</param>
        /// <returns>TRUE only if the shapes will have collided</returns>
        public bool WillCollide(Shape s1, Shape s2, Vector2 move)
        {
            //Runs the appropriate logic respective to the sub-class of the given Shapes
            if (s1 is Circle && s2 is Circle)
            {
                //Creates a copy to "simulate" the given shape's movement
                Circle newS1 = new Circle(s1.GetLocation() + move, s1.GetSize());

                //Checks for collision
                return this.HasCollided(newS1, (Circle)s2);
            }
            else if (s1 is Rect && s2 is Rect)
            {
                //Creates a copy to "simulate" the given shape's movement
                Rect newS1 = new Rect(s1.GetLocation() + move, ((Rect)(s1)).GetWidth(), ((Rect)(s1)).GetHeight());

                //Checks for collision
                return this.HasCollided(newS1, (Rect)s2);
            }
            else
            {
                //The Shape types don't match, return FALSE
                return false;
            }
        }

        /// <summary>
        /// Detects a predicted collision between two shapes in a given List of shapes, given a direction. (Given Shapes must be of the same sub-class)
        /// </summary>
        /// <param name="s">A valid Shape</param>
        /// <param name="shapeList">A valid Shape List</param>
        /// <param name="move">The direction in which the Shape "s1" will try to move</param>
        /// <returns>TRUE only if the given Shape cannot move around without colliding</returns>
        public bool WillCollide(Shape s, List<Shape> shapeList, Vector2 move)
        {
            //Runs the appropriate logic respective to the sub-class of the given Shapes
            if (s is Circle && this.ComprisedOfCircles(shapeList) == true)
            {
                //Creates a copy to "simulate" the given shape's movement
                Circle newShape = new Circle(s.GetLocation() + move, s.GetSize());

                //Checks for collisions with given objects
                foreach(Shape circle in shapeList)
                {
                    if(this.HasCollided(newShape, (Circle)circle) == true)
                    {
                        //Has found collision, returns TRUE
                        return true;
                    }
                }

                //No collision was found, return FALSE
                return false;
            }
            else if (s is Rect && this.ComprisedOfRects(shapeList) == true)
            {
                //Creates a copy to "simulate" the given shape's movement
                Rect newShape = new Rect(s.GetLocation() + move, ((Rect)(s)).GetWidth(), ((Rect)(s)).GetHeight());

                //Checks for collisions with given objects
                foreach(Shape rect in shapeList)
                {
                    if(this.HasCollided(newShape, (Rect)rect) == true)
                    {
                        //Has found collision, returns TRUE
                        return true;
                    }
                }

                //No collision was found, return FALSE
                return false;
            }
            else
            {
                //The Shape types don't match, return TRUE
                return true;
            }
        }

        /// <summary>
        /// Checks if the given List of Shapes is comprised of Circles
        /// </summary>
        /// <param name="shapeList">A valid List of Shapes</param>
        /// <returns>TRUE if all Shapes in list are Circles</returns>
        private bool ComprisedOfCircles(List<Shape> shapeList)
        {
            //Checks each Shape in shapeList
            foreach(Shape s in shapeList)
            {
                if (!(s is Circle))
                {
                    //A non-Circle shape has been found, return FALSE
                    return false;
                }
            }

            //No non-Circle Shape has been found, return TRUE
            return true;
        }

        /// <summary>
        /// Checks if the given List of Shapes is comprised of Rects
        /// </summary>
        /// <param name="shapeList">A valid List of Shapes</param>
        /// <returns>TRUE if all Shapes in list are Rects</returns>
        private bool ComprisedOfRects(List<Shape> shapeList)
        {
            //Checks each Shape in shapeList
            foreach (Shape s in shapeList)
            {
                if (!(s is Rect))
                {
                    //A non-Rect shape has been found, return FALSE
                    return false;
                }
            }

            //No non-Rect Shape has been found, return TRUE
            return true;
        }
    }
}
