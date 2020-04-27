//Author: Ilan Segal
//File Name: ShopMenu.cs
//Project Name: RoboKnight
//Creation Date: January 11, 2016
//Modified Date: January 12, 2016
//Description: Allows player to interact with the shop.

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
    class ShopMenu : ItemMenu
    {
        //Buy button
        private Button buyBtn;

        //Sound effect upon buying an item
        private SoundEffect buyEffect;

        /// <summary>
        /// Constructor for the ShopMenu class
        /// </summary>
        /// <param name="content">A valid ContentManager</param>
        /// <param name="stats">A valid ref PlayerStats object</param>
        public ShopMenu(ContentManager content, ref PlayerStats stats) : base(content, ref stats)
        {
            buyBtn = new Button(new Rectangle(600, BUTTON_HEIGHT * 6 + BUTTON_HEIGHT_OFFSET, BUTTON_WITDH, BUTTON_HEIGHT), "Buy for ", defaultPageBtnColour, new Vector2(600 + BUTTON_TEXT_WIDTH_OFFSET, BUTTON_HEIGHT * 6 + BUTTON_TEXT_HEIGHT_OFFSET + BUTTON_HEIGHT_OFFSET - 3));

            buyEffect = content.Load<SoundEffect>(@"Sound\Menu\buyitem");

            #region Loading Shop Menu
            this.AddItem(PassiveEffect.Health_LiIonBattery);
            this.AddItem(PassiveEffect.Health_FaradayArmour);
            this.AddItem(PassiveEffect.Health_NuclearPower);
            this.AddItem(PassiveEffect.Health_Chess);
            this.AddItem(PassiveEffect.Range_SilphScope);
            this.AddItem(PassiveEffect.Speed_AluminumArmour);
            this.AddItem(PassiveEffect.Speed_LiquidCooling);
            this.AddItem(PassiveEffect.Damage_FocusLens);
            this.AddItem(PassiveEffect.Damage_SapphireCrystal);
            this.AddItem(PassiveEffect.Damage_Graphene);
            this.AddItem(PassiveEffect.Damage_Nanomaterial);
            this.AddItem(PassiveEffect.FireRate_RoboOnion);
            this.AddItem(PassiveEffect.FireRate_Overclocked);
            this.AddItem(PassiveEffect.FireRate_TitanX);
            this.AddItem(PassiveEffect.Homing_HAL);
            this.AddItem(PassiveEffect.QuadShot_QuadCore);
            #endregion
        }

        /// <summary>
        /// Gets all buttons on this menu
        /// </summary>
        /// <returns>A List of Buttons</returns>
        public override List<Button> GetAllButtons()
        {
            return base.GetAllButtons().Concat(new List<Button> { buyBtn }).ToList();
        }

        /// <summary>
        /// Updates this ShopMenu
        /// </summary>
        /// <param name="ms">A valid MouseState</param>
        public override void Update(MouseState ms)
        {
            base.Update(ms);
            
            //Shows player how much they can buy an item for
            buyBtn.SetText("Buy for " + items[selectedItemIndex].GetValue() + "G");

            if (playerStats.GetGold() < items[selectedItemIndex].GetValue())
            {
                //Indicates that an item cannot be bought
                buyBtn.SetColour(inactiveColour);
            }
            else if (buyBtn.Hover(ms))
            {
                buyBtn.SetColour(hoverPageBtnColour);

                if (buyBtn.WasClicked(ms))
                {
                    //Buys effect
                    playerStats.SetGold(playerStats.GetGold() - items[selectedItemIndex].GetValue());
                    playerStats.AddEffect(items[selectedItemIndex].GetEffect());

                    //Plays sound effect
                    buyEffect.Play();

                    //Fully heals player if the bought item was a health item
                    if (items[selectedItemIndex].GetEffect() == PassiveEffect.Health_Chess ||
                        items[selectedItemIndex].GetEffect() == PassiveEffect.Health_FaradayArmour ||
                        items[selectedItemIndex].GetEffect() == PassiveEffect.Health_LiIonBattery ||
                        items[selectedItemIndex].GetEffect() == PassiveEffect.Health_NuclearPower)
                    {
                        playerStats.SetHealth(playerStats.GetMaxHealth());
                    }
                }
            }
            else
            {
                buyBtn.SetColour(defaultPageBtnColour);
            }
        }
    }
}
