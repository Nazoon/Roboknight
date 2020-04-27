//Author: Ilan Segal
//File Name: InvMenu.cs
//Project Name: RoboKnight
//Creation Date: January 12, 2016
//Modified Date: January 12, 2016
//Description: Allows player to interact with their inventory.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Roboknight
{
    class InvMenu : ItemMenu
    {
        //Sell button
        private Button sellBtn;

        //Sound effect upon selling an item
        private SoundEffect sellEffect;

        /// <summary>
        /// Constructor for the InvMenu class
        /// </summary>
        /// <param name="content">A valid ContentManager</param>
        /// <param name="stats">A valid ref PlayerStats object</param>
        public InvMenu(ContentManager content, ref PlayerStats stats) : base(content, ref stats)
        {
            sellBtn = new Button(new Rectangle(600, BUTTON_HEIGHT * 6 + BUTTON_HEIGHT_OFFSET, BUTTON_WITDH, BUTTON_HEIGHT), "Sell for ", defaultPageBtnColour, new Vector2(600 + BUTTON_TEXT_WIDTH_OFFSET, BUTTON_HEIGHT * 6 + BUTTON_TEXT_HEIGHT_OFFSET + BUTTON_HEIGHT_OFFSET - 3));

            sellEffect = content.Load<SoundEffect>(@"Sound\Menu\sellitem");
        }

        /// <summary>
        /// Gets all buttons on this menu
        /// </summary>
        /// <returns>A List of Buttons</returns>
        public override List<Button> GetAllButtons()
        {
            return base.GetAllButtons().Concat(new List<Button> { sellBtn }).ToList();
        }

        /// <summary>
        /// Updates this InvMenu
        /// </summary>
        /// <param name="ms">A valid MouseState</param>
        public override void Update(MouseState ms)
        {
            base.Update(ms);

            //Shows player how much they can sell their item for
            sellBtn.SetText("Sell for " + items[selectedItemIndex].GetValue() / 2 + "G");

            //Indicates that an item cannot be sold
            if (playerStats.GetPassiveEffects().Count == 1)
            {
                sellBtn.SetColour(inactiveColour);
            }
            else if (sellBtn.Hover(ms))
            {
                sellBtn.SetColour(hoverPageBtnColour);

                if (sellBtn.WasClicked(ms))
                {
                    //Gives the player money for selling their item
                    playerStats.SetGold(playerStats.GetGold() + items[selectedItemIndex].GetValue() / 2);

                    //Removes the item and shifts around index values if necessary
                    playerStats.GetPassiveEffects().RemoveAt(selectedItemIndex);
                    if (selectedItemIndex == playerStats.GetPassiveEffects().Count)
                    {
                        selectedItemIndex--;
                    }

                    if (pageIndex == playerStats.GetPassiveEffects().Count)
                    {
                        pageIndex -= MAX_ITEMS_ON_SCREEN;
                    }

                    //Plays sound effect
                    sellEffect.Play();
                }
            }
            else
            {
                sellBtn.SetColour(defaultPageBtnColour);
            }
        }

    }
}
