//Author: Ilan Segal
//File Name: Enemy.cs
//Project Name: RoboKnight
//Creation Date: December 25, 2015
//Modified Date: January 16, 2016
//Description: Class which represents the generic template for an enemy

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Roboknight
{
    class Enemy
    {
        //Sound effect data
        private ContentManager content;
        private SoundEffect growlEffect_0;
        private SoundEffect growlEffect_1;
        private SoundEffect shootEffect;

        //Data dealing with path-finding and following
        protected Path path;
        protected CollisionManager cm;
        protected Vector2 playerPrevLocation;
        protected List<Shape> roomObstacleList;
        protected const double DESTINATION_LENIENCY = 5;

        //Data dealing with movement and location
        protected Vector2 location;
        protected Shape obstacleHitbox;
        protected Shape projectileHitbox;

        //Stats
        protected double health;
        protected double speed;
        protected double projectileDamage;
        protected double contactDamage;
        protected double shotSpeed;
        protected double range;
        protected int fireRate;
        protected int timeSinceLastShot;
        protected double timeUntilNextRefresh;
        protected const double PATH_REFRESH_RATE = 200;

        //TRUE if this enemy deals damage upon touching player, otherwise FALSE
        protected bool dealsContactDamage;

        /// <summary>
        /// Constructor for the enemy class
        /// </summary>
        /// <param name="content">A valid ContentManager</param>
        /// <param name="location">Starting location of the enemy</param>
        public Enemy(ContentManager content, Vector2 location)
        {
            this.content = content;

            growlEffect_0 = this.content.Load<SoundEffect>(@"Sound\Enemy\growl1");
            growlEffect_1 = this.content.Load<SoundEffect>(@"Sound\Enemy\growl2");
            shootEffect = this.content.Load<SoundEffect>(@"Sound\Enemy\shoot");

            this.location = location;
        }

        /// <summary>
        /// Updates this enemy
        /// </summary>
        /// <param name="projectileField">Projectiles currently on screen</param>
        /// <param name="playerLocation">Player's current location</param>
        /// <param name="playerSize">The side length of the player's hitbox</param>
        /// <param name="gameTime">A valid GameTime</param>
        public virtual void Update(ref List<Projectile> projectileField, Vector2 playerLocation, GameTime gameTime)
        {
            //Randomly selects a "growl" noise every so often
            switch (new Random().Next(0, 501))
            {
                case 0:

                    growlEffect_0.Play(0.5F, -1F, 0F);
                    break;

                case 1:

                    growlEffect_1.Play(0.5F, -1F, 0F);
                    break;
            }
        }

        /// <summary>
        /// Gets the location
        /// </summary>
        /// <returns>This enemy's location</returns>
        public Vector2 GetLocation()
        {
            return location;
        }

        /// <summary>
        /// Gets the obstacle hitbox
        /// </summary>
        /// <returns>A Rect object</returns>
        public Shape GetObstacleHitbox()
        {
            return obstacleHitbox;
        }

        /// <summary>
        /// Gets the projectile hitbox
        /// </summary>
        /// <returns>A Circle object</returns>
        public Shape GetProjectileHitbox()
        {
            return projectileHitbox;
        }

        /// <summary>
        /// Gets the amount of damage that this enemy does upon contact
        /// </summary>
        /// <returns>A double variable</returns>
        public double GetContactDamage()
        {
            return contactDamage;
        }

        /// <summary>
        /// Indicates if this enemy deals damage upon contact
        /// </summary>
        /// <returns>TRUE if this enemy deals contact damage, otherwise FALSE</returns>
        public bool DealsContactDamage()
        {
            return dealsContactDamage;
        }

        public void SetObstacles(List<Obstacle> roomObstacles)
        {
            //Some enemies do not use the roomObstacleList, so make sure they have it before using it
            if (roomObstacleList != null)
            {
                //Updates collision detection
                roomObstacleList.Clear();
                foreach (Obstacle obstacle in roomObstacles)
                {
                    roomObstacleList.Add(obstacle.GetHitbox());
                }
            }
        }

        /// <summary>
        /// Deals damage to this enemy
        /// </summary>
        /// <param name="damage">A valid double</param>
        public void TakeDamage(double damage)
        {
            health -= damage;
        }

        /// <summary>
        /// Indicated if this enemy has been killed
        /// </summary>
        /// <returns>FALSE if health is above 0, otherwise TRUE</returns>
        public bool IsDead()
        {
            return (health <= 0);
        }

        /// <summary>
        /// Fires a projectile
        /// </summary>
        /// <param name="playerLocation">The location of the player</param>
        /// <param name="projectileField">A valid ref List of Projectiles</param>
        protected void FireProjectile(Vector2 playerLocation, ref List<Projectile> projectileField)
        {
            //Updates the projectile cooldown timer
            timeSinceLastShot = fireRate;

            //Calculates direction to fire projectile in
            double xDif = projectileHitbox.GetCenter().X - playerLocation.X;
            double yDif = projectileHitbox.GetCenter().Y - playerLocation.Y;
            double projectileAngle = Math.Atan(yDif / xDif);
            if (xDif > 0)
            {
                //Accounts for the player being behind the enemy on the x-axis
                projectileAngle += Math.PI;
            }
            
            //Determining colour for projectile, to indicate damage
            Color projectileColour;
            if (projectileDamage < 1)
            {
                projectileColour = Color.Crimson;
            }
            else
            {
                projectileColour = Color.DarkRed;
            }

            //Adds projectile to those currently on screen
            projectileField.Add(new Projectile(projectileAngle, shotSpeed, range, projectileHitbox.GetCenter(), projectileDamage, 60, projectileColour, "enemy"));

            //Plays sound effect
            shootEffect.Play(0.5F, 0F, 0F);
        }

        /// <summary>
        /// Refreshes this enemy's path
        /// </summary>
        /// <param name="playerLocation">The enemy's location</param>
        protected void RefreshPath(Vector2 playerLocation)
        {
            //Resets refesh cooldown counter
            timeUntilNextRefresh = PATH_REFRESH_RATE;

            //Refreshes path data
            path.SetStart(this.location);
            path.SetEnd(playerLocation);
            path.CalculatePath();
            playerPrevLocation = playerLocation;
        }

        protected void FollowPath()
        {
            if (path.GetPath().Count > 0)
            {
                //Declaring data
                double xDif;
                double yDif;
                double angle;
                Vector2 move = new Vector2();

                //Determining direction in which to move
                xDif = location.X - path.GetPath().Peek().GetLocation().X;
                yDif = location.Y - path.GetPath().Peek().GetLocation().Y;

                //Difference along the X-axis is NOT 0
                if (xDif != 0)
                {
                    angle = Math.Atan(yDif / xDif);
                    if (xDif > 0)
                    {
                        angle += Math.PI;
                    }

                    //Preparing the move vector
                    move = new Vector2((float)(speed * Math.Cos(angle)), (float)(speed * Math.Sin(angle)));
                }
                //Difference along the X-axis is 0
                else
                {
                    //To move down or up
                    if (yDif < 0)
                    {
                        move = new Vector2(0, (float)speed);
                    }
                    else if (yDif > 0)
                    {
                        move = new Vector2(0, (float)-speed);
                    }
                }

                //Moves enemy in direction if possible
                if (cm.WillCollide(obstacleHitbox, roomObstacleList, new Vector2(move.X, 0)) == false)
                {
                    location += new Vector2(move.X, 0);
                }
                if (cm.WillCollide(obstacleHitbox, roomObstacleList, new Vector2(0, move.Y)) == false)
                {
                    location += new Vector2(0, move.Y);
                }
                
                //Snaps enemy to destination if close enough
                if (Math.Abs(xDif) <= DESTINATION_LENIENCY && Math.Abs(yDif) <= DESTINATION_LENIENCY)
                {
                    location = path.GetPath().Pop().GetLocation();
                }

                //Updates hitboxes
                projectileHitbox.SetLocation(this.location);
                obstacleHitbox.SetLocation(this.location);
            }
        }
    }
}
