//Author: Ilan Segal
//File Name: ItemMenu.cs
//Project Name: RoboKnight
//Creation Date: January 10, 2016
//Modified Date: January 17, 2016
//Description: Allows player to interact with a dropdown menu of items.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Roboknight
{
    class ItemMenu
    {
        //Item data
        protected List<MenuItem> items;
        protected PlayerStats playerStats;
        protected int selectedItemIndex;

        //Colours for the page buttons
        protected Color defaultPageBtnColour;
        protected Color hoverPageBtnColour;
        protected Color inactiveColour;

        //Colours for the item buttons
        private Color defaultItemBtnColour;
        private Color selectedItemBtnColour;

        //Page-browsing buttons
        private Button pageUp;
        private Button pageDown;

        //Sound effects
        private SoundEffect clickEffect;
        private SoundEffect pageScrollEffect;

        //Data used for managing pages
        protected int pageIndex;
        protected const int MAX_ITEMS_ON_SCREEN = 5;

        //Data used to draw buttons
        protected const int BUTTON_WITDH = 480;
        protected const int BUTTON_HEIGHT = 60;
        protected const int BUTTON_WIDTH_OFFSET = 50;
        protected const int BUTTON_HEIGHT_OFFSET = 180;
        protected const int BUTTON_TEXT_HEIGHT_OFFSET = 10;
        protected const int BUTTON_TEXT_WIDTH_OFFSET = 35;

        /// <summary>
        /// Constructor for the ItemMenu class
        /// </summary>
        /// <param name="content">A valid ContentManager</param>
        /// <param name="stats">A valid ref PlayerStats</param>
        public ItemMenu(ContentManager content, ref PlayerStats stats)
        {
            items = new List<MenuItem>();
            playerStats = stats;

            defaultPageBtnColour = Color.Goldenrod;
            hoverPageBtnColour = Color.LightGoldenrodYellow;
            inactiveColour = Color.SlateGray;

            defaultItemBtnColour = Color.Red;
            selectedItemBtnColour = Color.DarkRed;

            pageUp = new Button(new Rectangle(BUTTON_WIDTH_OFFSET, BUTTON_HEIGHT_OFFSET, BUTTON_WITDH, BUTTON_HEIGHT), "^", defaultPageBtnColour, new Vector2(230 + BUTTON_WIDTH_OFFSET, BUTTON_TEXT_HEIGHT_OFFSET + BUTTON_HEIGHT_OFFSET + 5));
            pageDown = new Button(new Rectangle(BUTTON_WIDTH_OFFSET, BUTTON_HEIGHT * (MAX_ITEMS_ON_SCREEN + 1) + BUTTON_HEIGHT_OFFSET, BUTTON_WITDH, BUTTON_HEIGHT), "v", defaultPageBtnColour, new Vector2(225 + BUTTON_WIDTH_OFFSET, BUTTON_HEIGHT * (MAX_ITEMS_ON_SCREEN + 1) + BUTTON_TEXT_HEIGHT_OFFSET - 4 + BUTTON_HEIGHT_OFFSET));

            clickEffect = content.Load<SoundEffect>(@"Sound\Menu\button");
            pageScrollEffect = content.Load<SoundEffect>(@"Sound\Menu\pagescroll");

            pageIndex = 0;
        }

        /// <summary>
        /// Updates item list (To be used only with inventory menu)
        /// </summary>
        public void UpdateItemList()
        {
            //Updates item list
            items.Clear();
            foreach (PassiveEffect pe in playerStats.GetPassiveEffects())
            {
                this.AddItem(pe);
            }
        }

        /// <summary>
        /// Updates this ItemMenu
        /// </summary>
        /// <param name="ms">A valid MouseState</param>
        public virtual void Update(MouseState ms)
        {
            //Button cannot currently be used
            pageUp.SetColour(inactiveColour);
            pageDown.SetColour(inactiveColour);

            //Updates page buttons
            if (pageIndex - MAX_ITEMS_ON_SCREEN >= 0)
            {
                //Button is being hovered over
                if (pageUp.Hover(ms) == true)
                {
                    pageUp.SetColour(hoverPageBtnColour);

                    //Checks if the button is being pressed
                    if (pageUp.WasClicked(ms) == true)
                    {
                        //Scroll up
                        pageIndex -= MAX_ITEMS_ON_SCREEN;

                        //Play sound effect
                        pageScrollEffect.Play();
                    }
                }
                else
                {
                    //Button can be used
                    pageUp.SetColour(defaultPageBtnColour);
                }
            }
            if (pageIndex + MAX_ITEMS_ON_SCREEN < items.Count)
            {
                //Button is being hovered over
                if (pageDown.Hover(ms) == true)
                {
                    pageDown.SetColour(hoverPageBtnColour);

                    //Checks if the button is being pressed
                    if (pageDown.WasClicked(ms) == true)
                    {
                        //Scroll down
                        pageIndex += MAX_ITEMS_ON_SCREEN;

                        //Play sound effect
                        pageScrollEffect.Play();
                    }
                }
                else
                {
                    //Button can be used
                    pageDown.SetColour(defaultPageBtnColour);
                }
            }

            //Checks any of the buttons on screen in case they were clicked
            for(int i = 0; i < this.GetCurrentPageButtons().Count; i++)
            {
                if (this.GetCurrentPageButtons()[i].WasClicked(ms) == true)
                {
                    //If the current Button in this.GetCurrentPageButtons() was clicked, select the corresponding item in the Item List
                    selectedItemIndex = i + pageIndex;

                    //Play sound effect
                    clickEffect.Play();
                }
            }
        }

        /// <summary>
        /// Gets the MenuItems currently on screen
        /// </summary>
        /// <returns>A List of all MenuItems on the current page</returns>
        public List<MenuItem> GetCurrentPageItems()
        {
            List<MenuItem> returnList = new List<MenuItem>();

            //Collects all items to be managed on the current page
            for(int i = pageIndex; i < items.Count && i - pageIndex < MAX_ITEMS_ON_SCREEN; i++)
            {
                returnList.Add(items[i]);
            }

            return returnList;
        }

        /// <summary>
        /// Gets the Buttons currently on screen
        /// </summary>
        /// <returns>A List of all Buttons on the current page</returns>
        public List<Button> GetCurrentPageButtons()
        {
            List<Button> returnList = new List<Button>();

            //Collects all buttons to be managed on the current page
            for (int i = 0; i < this.GetCurrentPageItems().Count; i++)
            {
                //Determines colour by checking if the corresponding MenuItem is currently selected
                Color btnColour;
                if (i + pageIndex == selectedItemIndex)
                {
                    btnColour = selectedItemBtnColour;
                }
                else
                {
                    btnColour = defaultItemBtnColour;
                }

                //Adds the Button to the return List
                returnList.Add(new Button(new Rectangle(BUTTON_WIDTH_OFFSET, BUTTON_HEIGHT_OFFSET + BUTTON_HEIGHT * (i + 1), BUTTON_WITDH, BUTTON_HEIGHT),
                                this.GetCurrentPageItems()[i].GetName(),
                                btnColour,
                                new Vector2(BUTTON_WIDTH_OFFSET + BUTTON_TEXT_WIDTH_OFFSET, BUTTON_HEIGHT_OFFSET + BUTTON_HEIGHT * (i + 1) + BUTTON_TEXT_HEIGHT_OFFSET)));
            }

            return returnList;
        }

        /// <summary>
        /// Gets all buttons
        /// </summary>
        /// <returns>A List of Buttons</returns>
        public virtual List<Button> GetAllButtons()
        {
            List<Button> returnList = new List<Button>();

            returnList.Add(pageUp);
            returnList.Add(pageDown);
            returnList = (returnList.Concat(this.GetCurrentPageButtons())).ToList<Button>();

            return returnList;
        }

        /// <summary>
        /// Gets the PageUp button
        /// </summary>
        /// <returns>A Button object</returns>
        public Button GetPageUpBtn()
        {
            return pageUp;
        }

        /// <summary>
        /// Gets the PageDown button
        /// </summary>
        /// <returns>A Button object</returns>
        public Button GetPageDownBtn()
        {
            return pageDown;
        }

        /// <summary>
        /// Gets the currently selected MenuItem
        /// </summary>
        /// <returns>A MenuItem object</returns>
        public MenuItem GetCurrentItem()
        {
            return items[selectedItemIndex];
        }

        /// <summary>
        /// Adds an effect to the inventory
        /// </summary>
        /// <param name="pe">A valid PassiveEffect</param>
        protected void AddItem(PassiveEffect pe)
        {
            switch (pe)
            {
                case PassiveEffect.Damage_FocusLens:

                    items.Add(new MenuItem(pe, "Focus Lens", "Damage Up", "Your focus needs more focus.", 30));
                    break;

                case PassiveEffect.Damage_Graphene:

                    items.Add(new MenuItem(pe, "Graphene", "Damage Up", "Highly conductive!", 50));
                    break;

                case PassiveEffect.Damage_Nanomaterial:

                    items.Add(new MenuItem(pe, "Nanomaterial", "Damage Up", "Let's get nanoscopic.", 10));
                    break;

                case PassiveEffect.Damage_SapphireCrystal:

                    items.Add(new MenuItem(pe, "Sapphire Crystal", "Damage Up", "It's blue. (A ba dee a ba die)", 15));
                    break;

                case PassiveEffect.FireRate_Overclocked:

                    items.Add(new MenuItem(pe, "Overclocked!", "Fire Rate Up", "If only you installed liquid cooling...", 165));
                    break;

                case PassiveEffect.FireRate_RoboOnion:

                    items.Add(new MenuItem(pe, "Robo-Onion", "Fire Rate Up, Damage Down", "Robots cry lasers, apparently.", 15));
                    break;

                case PassiveEffect.FireRate_TitanX:

                    items.Add(new MenuItem(pe, "Titan X", "Fire Rate Up", "You're no peasant.", 50));
                    break;

                case PassiveEffect.Health_Chess:

                    items.Add(new MenuItem(pe, "Chess", "Health Up", "How about a nice game of chess?", 10));
                    break;

                case PassiveEffect.Health_FaradayArmour:

                    items.Add(new MenuItem(pe, "Faraday Armour", "Health Up", "Like chainmail! For robots!", 10));
                    break;

                case PassiveEffect.Health_LiIonBattery:

                    items.Add(new MenuItem(pe, "Li-Ion Battery", "Health Up", "It just goes on and on and on...", 10));
                    break;

                case PassiveEffect.Health_NuclearPower:

                    items.Add(new MenuItem(pe, "Nuclear Power", "Health Up", "Clean! (ish)", 10));
                    break;

                case PassiveEffect.Homing_HAL:

                    items.Add(new MenuItem(pe, "HAL", "Homing Lasers!", "I can't let you do that, Lane...", 40));
                    break;

                case PassiveEffect.QuadShot_QuadCore:

                    items.Add(new MenuItem(pe, "Quad Core", "Quad Shot! Fire Rate Down...", "Dodge this!", 45));
                    break;

                case PassiveEffect.Range_SilphScope:

                    items.Add(new MenuItem(pe, "Silph Scope", "Range Up", "Gotta see 'em all!", 30));
                    break;

                case PassiveEffect.Speed_AluminumArmour:

                    items.Add(new MenuItem(pe, "Aluminum Armour", "Speed Up", "Lighter casing.", 20));
                    break;

                case PassiveEffect.Speed_LiquidCooling:

                    items.Add(new MenuItem(pe, "Liquid Cooling", "Speed Up", "If only you were overclocked...", 165));
                    break;
            }
        }
    }
}
