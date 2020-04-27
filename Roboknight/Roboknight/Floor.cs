//Author: Ilan Segal
//File Name: Floor.cs
//Project Name: RoboKnight
//Creation Date: December 11, 2015
//Modified Date: January 16, 2016
//Description: Holds connections between rooms, which make up the map of the game.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class Floor
    {
        //To be passed into sequentially generated rooms
        private ContentManager content;

        //Allows an instance of this class to keep track of where its Rooms have generated, relative to the originRoom
        private bool[,] roomSwitch;
        private const bool FREE = true;
        private const bool TAKEN = false;

        //Room pointers
        private Room originRoom;
        private Room currentRoom;

        //Item pool data
        private List<PassiveEffect> availableItems;
        private List<PassiveEffect> closedItems;

        //Random number generator
        private Random rng;

        //Constants used during floor generation
        private const int ROOM_TYPES = 11;
        private const int ROOM_SIDES = 4;
        private const int UP = 0;
        private const int RIGHT = 1;
        private const int DOWN = 2;
        private const int LEFT = 3;

        /// <summary>
        /// Constructor for the Floor class
        /// </summary>
        /// <param name="content">A valid ContentManager</param>
        /// <param name="floorSize">Limit on how many Rooms wide or high this floor can be.</param>
        public Floor(ContentManager content, int floorSize)
        {
            this.content = content;

            availableItems = new List<PassiveEffect>();
            closedItems = new List<PassiveEffect>();

            #region Loading Item Pool
            availableItems.Add(PassiveEffect.Health_LiIonBattery);
            availableItems.Add(PassiveEffect.Health_FaradayArmour);
            availableItems.Add(PassiveEffect.Health_NuclearPower);
            availableItems.Add(PassiveEffect.Health_Chess);
            availableItems.Add(PassiveEffect.Range_SilphScope);
            availableItems.Add(PassiveEffect.Speed_AluminumArmour);
            availableItems.Add(PassiveEffect.Speed_LiquidCooling);
            availableItems.Add(PassiveEffect.Damage_FocusLens);
            availableItems.Add(PassiveEffect.Damage_SapphireCrystal);
            availableItems.Add(PassiveEffect.Damage_Graphene);
            availableItems.Add(PassiveEffect.Damage_Nanomaterial);
            availableItems.Add(PassiveEffect.FireRate_RoboOnion);
            availableItems.Add(PassiveEffect.FireRate_Overclocked);
            availableItems.Add(PassiveEffect.FireRate_TitanX);
            availableItems.Add(PassiveEffect.Homing_HAL);
            availableItems.Add(PassiveEffect.QuadShot_QuadCore);
            #endregion

            rng = new Random();

            //Array is created to allow up to the maximum size
            roomSwitch = new bool[floorSize, floorSize];

            //Sets each position on roomSwitch to FREE, meaning a room can generate there
            for (int y = 0; y < floorSize; y++)
            {
                for (int x = 0; x < floorSize; x++)
                {
                    roomSwitch[x, y] = FREE;
                }
            }

            //Starts origin room
            originRoom = new Room(this.content, 0);

            //Generates rooms off of the origin room
            roomSwitch[floorSize / 2, floorSize / 2] = TAKEN;
            this.GenerateRoom(originRoom, floorSize, floorSize / 2, floorSize / 2, 100);

            //Sets the current room to the origin room
            currentRoom = originRoom;
        }

        /// <summary>
        /// Gets the origin room
        /// </summary>
        /// <returns>The origin Room</returns>
        public Room GetOriginRoom()
        {
            return originRoom;
        }

        /// <summary>
        /// Gets the current room
        /// </summary>
        /// <returns>The room that the player is currently in</returns>
        public Room GetCurrentRoom()
        {
            return currentRoom;
        }

        /// <summary>
        /// Sets the current room
        /// </summary>
        /// <param name="r">A valid Room</param>
        public void SetCurrentRoom(Room r)
        {
            currentRoom = r;
        }

        /// <summary>
        /// Generates 0-4 rooms off of a given room
        /// </summary>
        /// <param name="stemRoom">The room to generate more rooms off of</param>
        /// <param name="size">The maximum size of the floor</param>
        /// <param name="xPos">X-value of the stem room on the roomSwitch array</param>
        /// <param name="yPos">Y-value of the stem room on the roomSwitch array</param>
        /// <param name="chance">Chance of trying to generate a room off of the stem room, out of 100</param>
        private void GenerateRoom(Room stemRoom, int size, int xPos, int yPos, int chance)
        {
            //Allows the program to know whether or not it can generate a room without overlapping with another one
            bool canGenerate;

            //Only tries to generate if the chance is high enough
            if (chance > 40)
            {
                //Generates for each side of the stem room
                for (int direction = UP; direction < ROOM_SIDES; direction++)
                {
                    //Begins with the assumption that a new room can be generated
                    canGenerate = true;

                    //Chance of trying is dependent on the given "chance" value
                    if (rng.Next(0, 101) <= chance)
                    {
                        //Sets canGenerate to TRUE if generating new room will not leave the bounds set by size
                        //Otherwise sets canGenerate to FALSE
                        switch (direction)
                        {
                            case UP:

                                canGenerate = (yPos > 0);
                                break;

                            case DOWN:

                                canGenerate = (yPos + 1 < size);
                                break;

                            case LEFT:

                                canGenerate = (xPos > 0);
                                break;

                            case RIGHT:

                                canGenerate = (xPos + 1 < size);
                                break;
                        }

                        //Only checks if it is still possible to generate a room
                        if (canGenerate == true)
                        {
                            //Keeps canGenerate set to TRUE if the position in roomSwitch corresponding to a new room is set to FREE
                            //Otherwise sets canGenerate to FALSE
                            switch (direction)
                            {
                                case UP:

                                    canGenerate = roomSwitch[xPos, yPos - 1];
                                    break;

                                case DOWN:

                                    canGenerate = roomSwitch[xPos, yPos + 1];
                                    break;

                                case LEFT:

                                    canGenerate = roomSwitch[xPos - 1, yPos];
                                    break;

                                case RIGHT:

                                    canGenerate = roomSwitch[xPos + 1, yPos];
                                    break;
                            }
                        }

                        //Only generates new room if both restrictions set above have been met
                        if (canGenerate == true)
                        {
                            //Generates new room
                            Room newRoom = new Room(content, rng.Next(1, ROOM_TYPES + 1));

                            //Sets new room to be connected to the stem room
                            stemRoom.SetConnectedRoom(direction, newRoom);

                            //Sets stem room to be connected to the new room
                            if (direction > 1)
                            {
                                newRoom.SetConnectedRoom(direction - 2, stemRoom);
                            }
                            else
                            {
                                newRoom.SetConnectedRoom(direction + 2, stemRoom);
                            }

                            //Incriments xPos or yPos depending on the direction that the new room was generated in
                            switch (direction)
                            {
                                case UP:

                                    //Marks the new room's corresponding position in roomSwitch to TAKEN, so no rooms overlap it
                                    roomSwitch[xPos, yPos - 1] = TAKEN;

                                    //Generates the room
                                    this.GenerateRoom(newRoom, size, xPos, yPos - 1, chance - 2);
                                    break;

                                case DOWN:

                                    //Marks the new room's corresponding position in roomSwitch to TAKEN, so no rooms overlap it
                                    roomSwitch[xPos, yPos + 1] = TAKEN;

                                    //Generates the room
                                    this.GenerateRoom(newRoom, size, xPos, yPos + 1, chance - 2);
                                    break;

                                case LEFT:

                                    //Marks the new room's corresponding position in roomSwitch to TAKEN, so no rooms overlap it
                                    roomSwitch[xPos - 1, yPos] = TAKEN;

                                    //Generates the room
                                    this.GenerateRoom(newRoom, size, xPos - 1, yPos, chance - 2);
                                    break;

                                case RIGHT:

                                    //Marks the new room's corresponding position in roomSwitch to TAKEN, so no rooms overlap it
                                    roomSwitch[xPos + 1, yPos] = TAKEN;

                                    //Generates the room
                                    this.GenerateRoom(newRoom, size, xPos + 1, yPos, chance - 2);
                                    break;
                            }
                        }
                    }
                }
            }
            //Turns the stem room into an item room
            else
            {
                //Clears all obstacles in the room
                stemRoom.GetObstacles().Clear();

                //Picking an item
                PassiveEffect item = availableItems[rng.Next(0, availableItems.Count)];

                //Takes the chosen item out of the item pool
                availableItems.Remove(item);
                closedItems.Add(item);

                //Places the item pedestal
                stemRoom.AddItemPedestal(item);

                //Refreshes the pool in the case that it gets drained of items
                if (availableItems.Count == 0)
                {
                    while (closedItems.Count > 0)
                    {
                        availableItems.Add(closedItems[0]);

                        closedItems.RemoveAt(0);
                    }
                }
            }


            //Builds walls around room
            stemRoom.BuildWalls();
        }
    }
}
