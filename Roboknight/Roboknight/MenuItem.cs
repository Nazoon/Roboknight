//Author: Ilan Segal
//File Name: MenuItem.cs
//Project Name: RoboKnight
//Creation Date: January 10, 2016
//Modified Date: January 12, 2016
//Description: Allows inv. and shop menus to keep track of item info.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class MenuItem
    {
        private PassiveEffect effect;

        private string name;
        private string effectDesc;
        private string desc;
        private int value;

        public MenuItem(PassiveEffect effect, string name, string effectDesc, string desc, int value)
        {
            this.effect = effect;
            this.name = name;
            this.effectDesc = effectDesc;
            this.desc = desc;
            this.value = value;
        }

        public PassiveEffect GetEffect()
        {
            return effect;
        }

        public string GetName()
        {
            return name;
        }

        public string GetEffectDesc()
        {
            return effectDesc;
        }

        public string GetItemDesc()
        {
            return desc;
        }

        public int GetValue()
        {
            return value;
        }
    }
}
