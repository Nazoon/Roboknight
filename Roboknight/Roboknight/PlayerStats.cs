//Author: Ilan Segal
//File Name: PlayerStats.cs
//Project Name: RoboKnight
//Creation Date: December 16, 2015
//Modified Date: January 13, 2016
//Description: Class which keeps track of a PlayerCharacter's stats

using System;
using System.Collections.Generic;
using System.Linq;

namespace Roboknight
{
    class PlayerStats
    {
        //Starting stats
        private const double BASE_DAMAGE = 30;
        private const double BASE_RANGE = 800;
        private const double BASE_SPEED = 10;
        private const double SHOT_SPEED = 15;
        private const int BASE_FIRERATE = 400;
        private const int FIRERATE_CAP = 50;
        private const double BASE_HEALTH = 5;

        //Player's health
        private double health;

        //Effects
        private List<PassiveEffect> passiveEffects;

        //Gold count
        private int gold;

        /// <summary>
        /// Constructor for the PlayerStats class
        /// </summary>
        public PlayerStats()
        {
            health = BASE_HEALTH;

            //The player starts with no effects
            this.passiveEffects = new List<PassiveEffect>();

            gold = 10;

            #region Giving the player a random effect at the beginning of the game
            switch (new Random().Next(0, 16))
            {
                case 0:

                    passiveEffects.Add(PassiveEffect.Damage_FocusLens);
                    break;

                case 1:

                    passiveEffects.Add(PassiveEffect.Damage_Graphene);
                    break;

                case 2:

                    passiveEffects.Add(PassiveEffect.Damage_Nanomaterial);
                    break;

                case 3:

                    passiveEffects.Add(PassiveEffect.Damage_SapphireCrystal);
                    break;

                case 4:

                    passiveEffects.Add(PassiveEffect.FireRate_Overclocked);
                    break;

                case 5:

                    passiveEffects.Add(PassiveEffect.FireRate_RoboOnion);
                    break;

                case 6:

                    passiveEffects.Add(PassiveEffect.FireRate_TitanX);
                    break;

                case 7:

                    passiveEffects.Add(PassiveEffect.Health_Chess);
                    break;

                case 8:

                    passiveEffects.Add(PassiveEffect.Health_FaradayArmour);
                    break;

                case 9:

                    passiveEffects.Add(PassiveEffect.Health_LiIonBattery);
                    break;

                case 10:

                    passiveEffects.Add(PassiveEffect.Health_NuclearPower);
                    break;

                case 11:

                    passiveEffects.Add(PassiveEffect.Homing_HAL);
                    break;

                case 12:

                    passiveEffects.Add(PassiveEffect.QuadShot_QuadCore);
                    break;

                case 13:

                    passiveEffects.Add(PassiveEffect.Range_SilphScope);
                    break;

                case 14:

                    passiveEffects.Add(PassiveEffect.Speed_AluminumArmour);
                    break;

                case 15:

                    passiveEffects.Add(PassiveEffect.Speed_LiquidCooling);
                    break;
            }
            #endregion
        }

        /// <summary>
        /// Adds a passive effect to the stats
        /// </summary>
        /// <param name="pe">A valid PassiveEffect</param>
        public void AddEffect(PassiveEffect pe)
        {
            passiveEffects.Add(pe);
        }

        /// <summary>
        /// Removes a specified effect from passiveEffects
        /// </summary>
        /// <param name="pe">A valid PassiveEffect</param>
        public void RemoveEffect(PassiveEffect pe)
        {
            passiveEffects.Remove(pe);
        }

        /// <summary>
        /// Gets all passive effects acting on the player
        /// </summary>
        /// <returns>A List of PassiveEffects</returns>
        public List<PassiveEffect> GetPassiveEffects()
        {
            return passiveEffects;
        }

        /// <summary>
        /// The Player's damage
        /// </summary>
        /// <returns>Damage, calculated with passive effects</returns>
        public double GetDamage()
        {
            //Begins with the base damage
            double damage = BASE_DAMAGE;

            //Greatens damage with each "Damage" passive effect in the passiveEffects list
            foreach (PassiveEffect pe in passiveEffects)
            {
                switch (pe)
                {
                    case PassiveEffect.Damage_FocusLens:

                        damage *= 1.9;
                        break;

                    case PassiveEffect.Damage_Graphene:

                        damage *= 3;
                        break;

                    case PassiveEffect.Damage_Nanomaterial:

                        damage += 5;
                        break;
                        
                    case PassiveEffect.Damage_SapphireCrystal:

                        damage += 10;
                        break;
                }
            }

            return damage;
        }

        /// <summary>
        /// The Player's projectile range
        /// </summary>
        /// <returns>Base range, or triple of base range if range up has been obtained</returns>
        public double GetRange()
        {
            //Returns 3x the base range if the Silph Scope has been obtained
            if (passiveEffects.Contains(PassiveEffect.Range_SilphScope))
            {
                return BASE_RANGE * 3;
            }

            //If the Silph Scope was not found in the list, return the base range
            return BASE_RANGE;
        }

        /// <summary>
        /// The Player's movement speed
        /// </summary>
        /// <returns>Speed, calculated with passive effects.</returns>
        public double GetSpeed()
        {
            double speed = BASE_SPEED;

            if (passiveEffects.Contains(PassiveEffect.Speed_AluminumArmour))
            {
                speed *= 1.4;
            }
            if (passiveEffects.Contains(PassiveEffect.Speed_LiquidCooling))
            {
                speed *= 1.1;
            }

            return speed;
        }

        /// <summary>
        /// The Player's shot's speed
        /// </summary>
        /// <returns>Shot speed, in pixels per update</returns>
        public double GetShotSpeed()
        {
            return SHOT_SPEED;
        }

        /// <summary>
        /// The milliseconds between firing of projectiles.
        /// </summary>
        /// <returns>Fire rate, calculated with passive effects. Lower limit is 100 ms.</returns>
        public int GetFireRate()
        {
            //Begins with the base firerate
            int firerate = BASE_FIRERATE;

            //Greatens firerate with each "FireRate" passive effect in the passiveEffects list
            foreach (PassiveEffect pe in passiveEffects)
            {
                switch (pe)
                {
                    case PassiveEffect.FireRate_Overclocked:

                        firerate = (int)(firerate / 1.1);
                        break;

                    case PassiveEffect.FireRate_TitanX:

                        firerate = (int)(firerate / 2.5);
                        break;

                    //Quad shot makes the player fire shots slower
                    case PassiveEffect.QuadShot_QuadCore:
                        
                        firerate *= 2;
                        break;
                        
                    case PassiveEffect.FireRate_RoboOnion:

                        firerate -= 200;
                        break;
                }
            }
            
            //Ensures the player's fire rate isn't ridiculously good, unless they've picked up the Overclocked power-up AND the Liquid Cooling power-up
            if(firerate < FIRERATE_CAP && !(passiveEffects.Contains(PassiveEffect.FireRate_Overclocked) && passiveEffects.Contains(PassiveEffect.Speed_LiquidCooling)))
            {
                return FIRERATE_CAP;
            }
            //Makes the player's fire rate very high, as long as they've picked up the Overclocked power-up AND the Liquid Cooling power-up
            else if (passiveEffects.Contains(PassiveEffect.FireRate_Overclocked) && passiveEffects.Contains(PassiveEffect.Speed_LiquidCooling))
            {
                return FIRERATE_CAP / 2;
            }

            //Neither conditions have been met, so return the normal fire rate
            return firerate;
        }

        /// <summary>
        /// Gets the player's health
        /// </summary>
        /// <returns>A double primitive</returns>
        public double GetHealth()
        {
            return health;
        }

        /// <summary>
        /// Gets the player's Max Health
        /// </summary>
        /// <returns>A double value</returns>
        public double GetMaxHealth()
        {
            //Starts with the base health
            double maxHealth = BASE_HEALTH;

            //Increments maxHealth for each Health-Up in passiveEffects
            foreach (PassiveEffect pe in passiveEffects)
            {
                if (pe == PassiveEffect.Health_Chess ||
                    pe == PassiveEffect.Health_FaradayArmour ||
                    pe == PassiveEffect.Health_LiIonBattery ||
                    pe == PassiveEffect.Health_NuclearPower)
                {
                    maxHealth++;
                }
            }

            return maxHealth;
        }

        /// <summary>
        /// Gets the player's gold count
        /// </summary>
        /// <returns>An int primitive</returns>
        public int GetGold()
        {
            return gold;
        }

        /// <summary>
        /// Sets the player's health
        /// </summary>
        /// <param name="newHealth">A valid double</param>
        public void SetHealth(double newHealth)
        {
            health = newHealth;
        }

        /// <summary>
        /// Sets the player's gold count
        /// </summary>
        /// <param name="newGold">A valid int</param>
        public void SetGold(int newGold)
        {
            gold = newGold;
        }

        /// <summary>
        /// Indicates if the Player's projectiles should be homing
        /// </summary>
        /// <returns>TRUE if the Skynet powerup has been obtained, FALSE otherwise</returns>
        public bool HasHomingShot()
        {
            return passiveEffects.Contains(PassiveEffect.Homing_HAL);
        }

        /// <summary>
        /// Indicates if the Player has Quad Shot
        /// </summary>
        /// <returns>TRUE if the QuadCore powerup has been obtained, FALSE otherwise</returns>
        public bool HasQuadShot()
        {
            return passiveEffects.Contains(PassiveEffect.QuadShot_QuadCore);
        }
    }
}
