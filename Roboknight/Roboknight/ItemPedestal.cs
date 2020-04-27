//Author: Ilan Segal
//File Name: ItemPedestal.cs
//Project Name: RoboKnight
//Creation Date: January 1, 2016
//Modified Date: January 1, 2016
//Description: Class which represents a power-up which the player can pick up

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class ItemPedestal
    {
        //Pedestal collision box
        private Shape hitbox;

        //The item which this pedestal contains
        private PassiveEffect item;

        //Standard size for the pedestal
        private const int PEDESTAL_SIZE = 150;

        //Used to determine center of room
        private const int ROOM_HEIGHT = 675;
        private const int ROOM_WIDTH = 1200;

        
        public ItemPedestal(PassiveEffect item)
        {
            hitbox = new Circle(new Vector2(ROOM_WIDTH / 2 - PEDESTAL_SIZE / 2, ROOM_HEIGHT / 2 - PEDESTAL_SIZE / 2), PEDESTAL_SIZE);
            this.item = item; 
        }

        /// <summary>
        /// Gets this pedestal's hitbox
        /// </summary>
        /// <returns>A Rect object</returns>
        public Shape GetHitbox()
        {
            return hitbox;
        }

        /// <summary>
        /// Gets the item which this pedestal holds
        /// </summary>
        /// <returns>A PassiveEffect</returns>
        public PassiveEffect GetItem()
        {
            return item;
        }

        /// <summary>
        /// Sets the item which this pedestal holds
        /// </summary>
        /// <param name="newItem">A valid PassiveEffect</param>
        public void SetItem(PassiveEffect newItem)
        {
            item = newItem;
        }
    }
}
