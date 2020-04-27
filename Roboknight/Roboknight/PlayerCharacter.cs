//Author: Ilan Segal
//File Name: PlayerCharacter.cs
//Project Name: RoboKnight
//Creation Date: December 5, 2015
//Modified Date: January 17, 2016
//Description: The class which represents the player's character.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Roboknight
{
    class PlayerCharacter
    {
        //Hitbox data
        private Shape obstacleHitBox;
        private Shape projectileHitBox;

        //General stat data
        private PlayerStats stats;
        private int cooldownTimer;
        private Vector2 location;

        //Sound effects
        private SoundEffect playerShootEffect;

        //Constants dealing with firing projectiles
        private const double RIGHT = 0;
        private const double DOWN = 0.5 * Math.PI;
        private const double LEFT = Math.PI;
        private const double UP = 1.5 * Math.PI;

        //Angles of shots when Quad Shot is acquired, used relative to the UP, DOWN, LEFT, and RIGHT angles
        private readonly double[] QUAD_ANGLE = new double[4] { 0.087 * Math.PI, 0.03 * Math.PI, -0.03 * Math.PI, -0.087 * Math.PI };

        //Size of the hitbox
        private const double BOX_SIZE = 60;

        /// <summary>
        /// Constructor for the PlayerCharacter class
        /// </summary>
        /// <param name="content">A valid ContentManager</param>
        /// <param name="startingLocation">Starting Vector2 location of the player</param>
        public PlayerCharacter(ContentManager content, Vector2 startingLocation)
        {
            //Setting up the hitbox
            obstacleHitBox = new Rect(startingLocation, BOX_SIZE, BOX_SIZE);
            projectileHitBox = new Circle(startingLocation, BOX_SIZE);

            //Setting up general stats
            stats = new PlayerStats();
            cooldownTimer = 0;
            location = startingLocation;

            //Loading sound effects
            playerShootEffect = content.Load<SoundEffect>(@"Sound\Player\playershoot");
        }

        /// <summary>
        /// Moves the player by specified amounts
        /// </summary>
        /// <param name="x">How many pixels the character will be moved along the x-axis</param>
        /// <param name="y">How many pixels the character will be moved along the y-axis</param>
        public void Move(double x, double y)
        {
            //Moves player
            location += new Vector2((float)x, (float)y);

            //Moves the hitbox to follow the player
            obstacleHitBox.SetLocation(location);
            projectileHitBox.SetLocation(location);
        }

        /// <summary>
        /// Attempts to fire a projectile
        /// </summary>
        /// <param name="list">A valid reference Projectile List to add a Projectile to</param>
        /// <param name="direction">Direction in which to shoot a Projectile</param>
        /// <param name="gameTime">A valid GameTime</param>
        public void FireProjectile(ref List<Projectile> list, WeaponState direction, GameTime gameTime)
        {
            //Updates the amount of time left before another projectile can be fired
            cooldownTimer -= gameTime.ElapsedGameTime.Milliseconds;

            //Fires a projectile only if the cooldown has worn off
            if (cooldownTimer <= 0)
            {
                if (direction != WeaponState.None)
                {
                    //Resets the cooldown timer
                    cooldownTimer = stats.GetFireRate();

                    //Plays firing sound effect
                    playerShootEffect.Play(0.1F, 1F, 0F);
                }

                switch (direction)
                {
                    //Fires projectile upwards
                    case WeaponState.Up:

                        if (stats.HasQuadShot() == false)
                        {
                            //Fires a single projectile
                            list.Add(new Projectile(UP, stats.GetShotSpeed(), stats.GetRange(), projectileHitBox.GetCenter(), stats.GetDamage(), Color.Red, "player"));
                        }
                        else
                        {
                            //Fires 4 projectiles, which are all pointing generally upwards
                            for (int i = 0; i < QUAD_ANGLE.Length; i++)
                            {
                                list.Add(new Projectile(UP + QUAD_ANGLE[i], stats.GetShotSpeed(), stats.GetRange(), projectileHitBox.GetCenter(), stats.GetDamage(), Color.Red, "player"));
                            }
                        }
                        
                        break;

                    //Fires projectile downwards
                    case WeaponState.Down:

                        if (stats.HasQuadShot() == false)
                        {
                            //Fires a single projectile
                            list.Add(new Projectile(DOWN, stats.GetShotSpeed(), stats.GetRange(), projectileHitBox.GetCenter(), stats.GetDamage(), Color.Red, "player"));
                        }
                        else
                        {
                            //Fires 4 projectiles, which are all pointing generally downwards
                            for (int i = 0; i < QUAD_ANGLE.Length; i++)
                            {
                                list.Add(new Projectile(DOWN + QUAD_ANGLE[i], stats.GetShotSpeed(), stats.GetRange(), projectileHitBox.GetCenter(), stats.GetDamage(), Color.Red, "player"));
                            }
                        }

                        break;

                    //Fires projectile to the left
                    case WeaponState.Left:

                        if (stats.HasQuadShot() == false)
                        {
                            //Fires a single projectile
                            list.Add(new Projectile(LEFT, stats.GetShotSpeed(), stats.GetRange(), projectileHitBox.GetCenter(), stats.GetDamage(), Color.Red, "player"));
                        }
                        else
                        {
                            for (int i = 0; i < QUAD_ANGLE.Length; i++)
                            {
                                //Fires 4 projectiles, which are all pointing generally to the left
                                list.Add(new Projectile(LEFT + QUAD_ANGLE[i], stats.GetShotSpeed(), stats.GetRange(), projectileHitBox.GetCenter(), stats.GetDamage(), Color.Red, "player"));
                            }
                        }

                        break;

                    //Fires projectile to the right
                    case WeaponState.Right:

                        if (stats.HasQuadShot() == false)
                        {
                            //Fires a single projectile
                            list.Add(new Projectile(RIGHT, stats.GetShotSpeed(), stats.GetRange(), projectileHitBox.GetCenter(), stats.GetDamage(), Color.Red, "player"));
                        }
                        else
                        {
                            for (int i = 0; i < QUAD_ANGLE.Length; i++)
                            {
                                //Fires 4 projectiles, which are all pointing generally to the right
                                list.Add(new Projectile(RIGHT + QUAD_ANGLE[i], stats.GetShotSpeed(), stats.GetRange(), projectileHitBox.GetCenter(), stats.GetDamage(), Color.Red, "player"));
                            }
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Adds a passive effect to the player's stats
        /// </summary>
        /// <param name="pe">A valid PassiveEffect</param>
        public void AddPassiveEffect(PassiveEffect pe)
        {
            stats.AddEffect(pe);
        }

        /// <summary>
        /// Gets the hitbox used for obstacles
        /// </summary>
        /// <returns>A Rect Shape object</returns>
        public Shape GetObstacleHitbox()
        {
            return obstacleHitBox;
        }

        /// <summary>
        /// Gets the hitbox used for projectiles
        /// </summary>
        /// <returns>A Circle Shape object</returns>
        public Shape GetProjectileHitbox()
        {
            return projectileHitBox;
        }

        /// <summary>
        /// Gets the player's stats
        /// </summary>
        /// <returns>A PlayerStats object</returns>
        public PlayerStats GetStats()
        {
            return stats;
        }

        /// <summary>
        /// Gets the player's location
        /// </summary>
        /// <returns>A Vector2 object</returns>
        public Vector2 GetLocation()
        {
            return location;
        }

        /// <summary>
        /// Sets the player's location
        /// </summary>
        /// <param name="newLocation">A valid Vector2 object</param>
        public void SetLocation(Vector2 newLocation)
        {
            location = newLocation;
            obstacleHitBox.SetLocation(location);
            projectileHitBox.SetLocation(location);
        }

        /// <summary>
        /// The player takes a given amount of damage
        /// </summary>
        /// <param name="damage">A valid double</param>
        public void TakeDamage(double damage)
        {
            stats.SetHealth(stats.GetHealth() - damage);
        }
    }
}
