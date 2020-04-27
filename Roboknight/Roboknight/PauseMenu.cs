//Author: Ilan Segal
//File Name: PauseMenu.cs
//Project Name: RoboKnight
//Creation Date: January 9, 2016
//Modified Date: April 13, 2016
//Description: Allows player to pause game and view map, stats, inventory, or shop.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Roboknight
{
    class PauseMenu
    {
        //Used to prevent double inputs
        private MouseState prevMs;

        //Current tab
        private PauseTab tab;

        //Interfaces for the various tabs
        private ItemMenu invMenu;
        private ItemMenu shopMenu;
        private Map map;

        //Constant for map scale
        private const int MAP_SCALE = 7;

        //Data used to render map
        private Room currentRoom;

        //Colour data
        private Color mapColour;
        private Color statsColour;
        private Color invColour;
        private Color shopColour;
        private Color inactiveColour;
        private Color hoverColour;

        //Buttons for switching between tabs
        private Button mapButton;
        private Button statsButton;
        private Button invButton;
        private Button shopButton;

        //Tab swtch sound effect
        private SoundEffect tabEffect;
        
        //Constants used for drawing buttons
        private const int BUTTON_WITDH = 120;
        private const int BUTTON_HEIGHT = 80;
        private const int BUTTON_WIDTH_OFFSET = 50;
        private const int BUTTON_HEIGHT_OFFSET = 100;
        private const int BUTTON_TEXT_OFFSET = 17;

        public PauseMenu(ContentManager content, Room currentRoom, ref PlayerStats stats)
        {
            prevMs = Mouse.GetState();

            tab = PauseTab.Stats;

            invMenu = new InvMenu(content, ref stats);
            shopMenu = new ShopMenu(content, ref stats);
            map = new Map();

            this.currentRoom = currentRoom;

            //Setting up button colours
            mapColour = Color.RoyalBlue;
            statsColour = Color.DarkRed;
            invColour = Color.DarkGreen;
            shopColour = Color.Gold;
            inactiveColour = Color.DarkSlateGray;
            hoverColour = Color.SlateGray;

            //Setting up buttons
            mapButton = new Button(new Rectangle(BUTTON_WITDH * 0 + BUTTON_WIDTH_OFFSET, BUTTON_HEIGHT_OFFSET, BUTTON_WITDH, BUTTON_HEIGHT), "Map", mapColour, new Vector2(BUTTON_WITDH * 0 + BUTTON_WIDTH_OFFSET + 22, BUTTON_HEIGHT_OFFSET + BUTTON_TEXT_OFFSET));
            statsButton = new Button(new Rectangle(BUTTON_WITDH * 1 + BUTTON_WIDTH_OFFSET, BUTTON_HEIGHT_OFFSET, BUTTON_WITDH, BUTTON_HEIGHT), "Stats", inactiveColour, new Vector2(BUTTON_WITDH * 1 + BUTTON_WIDTH_OFFSET + 8, BUTTON_HEIGHT_OFFSET + BUTTON_TEXT_OFFSET));
            invButton = new Button(new Rectangle(BUTTON_WITDH * 2 + BUTTON_WIDTH_OFFSET, BUTTON_HEIGHT_OFFSET, BUTTON_WITDH, BUTTON_HEIGHT), "Inv.", inactiveColour, new Vector2(BUTTON_WITDH * 2 + BUTTON_WIDTH_OFFSET + 23, BUTTON_HEIGHT_OFFSET + BUTTON_TEXT_OFFSET));
            shopButton = new Button(new Rectangle(BUTTON_WITDH * 3 + BUTTON_WIDTH_OFFSET, BUTTON_HEIGHT_OFFSET, BUTTON_WITDH, BUTTON_HEIGHT), "Shop", inactiveColour, new Vector2(BUTTON_WITDH * 3 + BUTTON_WIDTH_OFFSET + 12, BUTTON_HEIGHT_OFFSET + BUTTON_TEXT_OFFSET));

            //Setting up tab switch sound effect
            tabEffect = content.Load<SoundEffect>(@"Sound\Menu\tab");
        }

        /// <summary>
        /// Updates the PauseMenu
        /// </summary>
        /// <param name="ms">A valid MouseState</param>
        /// <param name="currentRoom">The players current room</param>
        public void Update(MouseState ms, Room currentRoom)
        {
            //Updates data that the menu must keep track of
            this.currentRoom = currentRoom;

            if (!ms.Equals(prevMs))
            {
                prevMs = ms;

                //Updates tab, depending on which button was clicked
                if (mapButton.WasClicked(ms) == true)
                {
                    tab = PauseTab.Map;

                    //Plays sound effect
                    tabEffect.Play();
                }
                else if (statsButton.WasClicked(ms) == true)
                {
                    tab = PauseTab.Stats;

                    //Plays sound effect
                    tabEffect.Play();
                }
                else if (invButton.WasClicked(ms) == true)
                {
                    tab = PauseTab.Inventory;

                    //Plays sound effect
                    tabEffect.Play();
                }
                else if (shopButton.WasClicked(ms) == true)
                {
                    tab = PauseTab.Shop;

                    //Plays sound effect
                    tabEffect.Play();
                }

                //Sets button colours to highlight the tab which is currently selected
                switch (tab)
                {
                    case PauseTab.Map:

                        mapButton.SetColour(mapColour);
                        statsButton.SetColour(inactiveColour);
                        invButton.SetColour(inactiveColour);
                        shopButton.SetColour(inactiveColour);

                        break;

                    case PauseTab.Stats:

                        mapButton.SetColour(inactiveColour);
                        statsButton.SetColour(statsColour);
                        invButton.SetColour(inactiveColour);
                        shopButton.SetColour(inactiveColour);

                        break;

                    case PauseTab.Inventory:

                        mapButton.SetColour(inactiveColour);
                        statsButton.SetColour(inactiveColour);
                        invButton.SetColour(invColour);
                        shopButton.SetColour(inactiveColour);

                        invMenu.UpdateItemList();
                        invMenu.Update(ms);

                        break;

                    case PauseTab.Shop:

                        mapButton.SetColour(inactiveColour);
                        statsButton.SetColour(inactiveColour);
                        invButton.SetColour(inactiveColour);
                        shopButton.SetColour(shopColour);

                        shopMenu.Update(ms);

                        break;
                }

                //Sets button colours to indicate if any are being hovered over
                if (mapButton.Hover(ms) == true && tab != PauseTab.Map)
                {
                    mapButton.SetColour(hoverColour);
                }
                else if (statsButton.Hover(ms) == true && tab != PauseTab.Stats)
                {
                    statsButton.SetColour(hoverColour);
                }
                else if (invButton.Hover(ms) == true && tab != PauseTab.Inventory)
                {
                    invButton.SetColour(hoverColour);
                }
                else if (shopButton.Hover(ms) == true && tab != PauseTab.Shop)
                {
                    shopButton.SetColour(hoverColour);
                }
            }
        }

        public void UpdateMap()
        {
            //Generates map
            map.GenerateMap(currentRoom, MAP_SCALE);
        }

        public PauseTab GetTab()
        {
            return tab;
        }

        public ItemMenu GetInvMenu()
        {
            return invMenu;
        }

        public ItemMenu GetShopMenu()
        {
            return shopMenu;
        }

        public string[,] GetMapData()
        {
            return map.GetMapData();
        }

        public void SetTab(PauseTab newTab)
        {
            tab = newTab;
        }

        public List<Button> GetAllButtons()
        {
            //Adds the tab buttons by default
            List<Button> returnList = new List<Button> {mapButton, statsButton, invButton, shopButton};

            switch (tab)
            {
                case PauseTab.Map:

                    break;

                case PauseTab.Stats:
                    
                    break;

                case PauseTab.Inventory:

                    returnList = returnList.Concat(invMenu.GetCurrentPageButtons()).ToList();
                    returnList = returnList.Concat(invMenu.GetAllButtons()).ToList();

                    break;

                case PauseTab.Shop:

                    returnList = returnList.Concat(shopMenu.GetCurrentPageButtons()).ToList();
                    returnList = returnList.Concat(shopMenu.GetAllButtons()).ToList();

                    break;
            }

            return returnList;
        }
    }
}
