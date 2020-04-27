//Author: Ilan Segal
//File Name: PathNode.cs
//Project Name: RoboKnight
//Creation Date: December 24, 2015
//Modified Date: January 4, 2016
//Description: The class which represents a single step in a Path

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class PathNode
    {
        //This node's location on a grid
        private Vector2 location;

        //The node that this node branches off from
        private PathNode parent;

        //Various costs of the node
        private int gCost;
        private int hCost;

        //Allows for easy standardization of obstacle locations and sizes
        private const int GRID_UNIT = 75;

        /// <summary>
        /// Constructor for the PathNode class
        /// </summary>
        /// <param name="location">A valid Vector2</param>
        /// <param name="parent">A valid PathNode</param>
        public PathNode(Vector2 location, PathNode parent)
        {
            this.location = location;
            this.parent = parent;

            //Set to -1 to indicate undefined cost
            gCost = -1;
        }

        /// <summary>
        /// Gets the location of this node
        /// </summary>
        /// <returns>This node's location</returns>
        public Vector2 GetLocation()
        {
            return location;
        }

        /// <summary>
        /// Gets the center location
        /// </summary>
        /// <returns>A Vector2 object</returns>
        public Vector2 GetCenter()
        {
            return new Vector2(location.X - GRID_UNIT / 2, location.Y - GRID_UNIT / 2);
        }

        /// <summary>
        /// Gets the parent node for this node
        /// </summary>
        /// <returns>This node's parent node</returns>
        public PathNode GetParent()
        {
            return parent;
        }

        /// <summary>
        /// Gets the G-Cost for this node
        /// </summary>
        /// <returns>This node's G-Cost</returns>
        public int GetGCost()
        {
            return gCost;
        }

        /// <summary>
        /// Gets the H-Cost for this node
        /// </summary>
        /// <returns>This node's H-Cost</returns>
        public int GetHCost()
        {
            return hCost;
        }

        /// <summary>
        /// Gets the F-Cost for this node
        /// </summary>
        /// <returns>This node's F-Cost</returns>
        public int GetFCost()
        {
            return gCost + hCost;
        }

        /// <summary>
        /// Sets this node's parent node
        /// </summary>
        /// <param name="newParent">A valid PathNode</param>
        public void SetParent(PathNode newParent)
        {
            parent = newParent;
        }

        /// <summary>
        /// Sets the G-Cost for this node
        /// </summary>
        /// <param name="newGCost">A valid int</param>
        public void SetGCost(int newGCost)
        {
            gCost = newGCost;
        }

        /// <summary>
        /// Sets the H-Cost for this node
        /// </summary>
        /// <param name="newHCost">A valid int</param>
        public void SetHCost(int newHCost)
        {
            hCost = newHCost;
        }
    }
}
