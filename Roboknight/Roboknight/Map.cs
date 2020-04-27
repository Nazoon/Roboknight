//Author: Ilan Segal
//File Name: Map.cs
//Project Name: RoboKnight
//Creation Date: April 13, 2016
//Modified Date: April 15, 2016
//Description: Represents the datastructures for a map

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class Map
    {
        //Map grid
        private string[,] roomGrid;

        //Used when managing rooms' positions relative to eachother
        private const int UP = 0;
        private const int RIGHT = 1;
        private const int DOWN = 2;
        private const int LEFT = 3;
        private const int NO_DIRECTION = -1;

        /// <summary>
        /// Constructor for the Map class
        /// </summary>
        public Map()
        {
        }

        /// <summary>
        /// Gets the map data
        /// </summary>
        /// <returns>A 2D string array containing all data for the map</returns>
        public string[,] GetMapData()
        {
            return roomGrid;
        }

        /// <summary>
        /// Generates a new map
        /// </summary>
        /// <param name="curPlayerRoom">The Room that the player is currently in</param>
        /// <param name="gridSize">The maximum side length of the square map (must be odd for best results)</param>
        public void GenerateMap(Room curPlayerRoom, int gridSize)
        {
            //Remakes the grid
            roomGrid = new string[gridSize, gridSize];

            //Initially sets all values blank
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    roomGrid[x, y] = "";
                }
            }

            //Generates map
            this.GenerateMap(curPlayerRoom, gridSize / 2, gridSize / 2, NO_DIRECTION);
        }

        /// <summary>
        /// Recursively generates map
        /// </summary>
        /// <param name="curSubprogramRoom">The given Room to check for connected rooms</param>
        /// <param name="x">The x-pos of the given Room on the roomGrid</param>
        /// <param name="y">The y-pos of the given Room on the roomGrid</param>
        /// <param name="forbiddenDirection">Direction which thed program must not check in</param>
        private void GenerateMap(Room curSubprogramRoom, int x, int y, int forbiddenDirection)
        {

            //Checks for a room to the north of the given Room
            if (curSubprogramRoom.GetConnectedRoom(UP) != null)
            {
                roomGrid[x, y] += '1';

                //Only runs if adjacent room has not been catalogued
                if (forbiddenDirection != UP)
                {
                    //Runs the program again as long as it does not go out of bounds
                    if (y > 0)
                    {
                        this.GenerateMap(curSubprogramRoom.GetConnectedRoom(UP), x, y - 1, DOWN);
                    }
                }
            }
            //Checks for a room to the east of the given Room
            if (curSubprogramRoom.GetConnectedRoom(RIGHT) != null)
            {
                roomGrid[x, y] += '2';
                
                //Only runs if adjacent room has not been catalogued
                if (forbiddenDirection != RIGHT)
                {
                    //Runs the program again as long as it does not go out of bounds
                    if (x + 1 < roomGrid.GetLength(0))
                    {
                        this.GenerateMap(curSubprogramRoom.GetConnectedRoom(RIGHT), x + 1, y, LEFT);
                    }
                }
            }
            //Checks for a room to the south of the given Room
            if (curSubprogramRoom.GetConnectedRoom(DOWN) != null)
            {
                roomGrid[x, y] += '3';
                
                //Only runs if adjacent room has not been catalogued
                if (forbiddenDirection != DOWN)
                {
                    //Runs the program again as long as it does not go out of bounds
                    if (y + 1 < roomGrid.GetLength(1))
                    {
                        this.GenerateMap(curSubprogramRoom.GetConnectedRoom(DOWN), x, y + 1, UP);
                    }
                }
            }
            //Checks for a room to the west of the given Room
            if (curSubprogramRoom.GetConnectedRoom(LEFT) != null)
            {
                roomGrid[x, y] += '4';
                
                //Only runs if adjacent room has not been catalogued
                if (forbiddenDirection != LEFT)
                {
                    //Runs the program again as long as it does not go out of bounds
                    if (x > 0)
                    {
                        this.GenerateMap(curSubprogramRoom.GetConnectedRoom(LEFT), x - 1, y, RIGHT);
                    }
                }
            }
        }
    }
}
