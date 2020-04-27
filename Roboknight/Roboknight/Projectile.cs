//Author: Ilan Segal
//File Name: Projectile.cs
//Project Name: RoboKnight
//Creation Date: December 15, 2015
//Modified Date: January 15, 2016
//Description: The class which represents a projectile.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class Projectile
    {
        //Used to generate random attributes for particles
        private Random rng;

        //Data dealing with trajectory and travel of projectile
        private double angle;
        private double speed;
        private double range;
        private double distanceTraveled;

        //Used to check if the projectile is touching an object
        private Shape hurtbox;
        private Shape hitbox;

        //Set value for projectile hitbox size
        private const double HITBOX_SIZE = 20;

        //The damage that the projectile carries with it
        private double damage;

        //The colour that the projectile is to be drawn with
        private Color colour;

        //Used to distinguish enemy projectiles from friendly projectiles
        private string type;

        /// <summary>
        /// Constructor for the Projectile class, where the projectile size must be given
        /// </summary>
        /// <param name="angle">The angle at which the projectile will fly, 0 and 360 being straight to the right</param>
        /// <param name="speed">The pixels per update that the projectile will move at</param>
        /// <param name="range">The maximum distance this projectile can fly</param>
        /// <param name="startingLocation">A valid Vector2</param>
        /// <param name="damage">The damage that this projectile deals upon impact</param>
        /// <param name="size">Size of the projectile</param>
        /// <param name="colour">A valid Color</param>
        /// <param name="type">A valid string</param>
        public Projectile(double angle, double speed, double range, Vector2 startingLocation, double damage, double size, Color colour, string type)
        {
            rng = new Random();

            this.angle = angle;
            this.speed = speed;
            this.range = range;
            this.distanceTraveled = 0;

            //Creates a hurtbox and hitbox for this projectile
            this.hurtbox = new Circle(startingLocation, size);
            this.hitbox = new Rect(startingLocation, HITBOX_SIZE, HITBOX_SIZE);

            //Sets proper position for projectile, relative to center of sender
            hurtbox.SetCenter(startingLocation);
            hitbox.SetCenter(startingLocation);

            this.damage = damage;

            this.colour = colour;

            this.type = type;
        }

        /// <summary>
        /// Constructor for the Projectile class, where the projectile size is calculated automatically
        /// </summary>
        /// <param name="angle">The angle at which the projectile will fly, 0 and 360 being straight to the right</param>
        /// <param name="speed">The pixels per update that the projectile will move at</param>
        /// <param name="range">The maximum distance this projectile can fly</param>
        /// <param name="startingLocation">A valid Vector2</param>
        /// <param name="damage">The damage that this projectile deals upon impact</param>
        /// <param name="colour">A valid Color</param>
        /// <param name="type">A valid string</param>
        public Projectile(double angle, double speed, double range, Vector2 startingLocation, double damage, Color colour, string type)
        {
            rng = new Random();

            this.angle = angle;
            this.speed = speed;
            this.range = range;
            distanceTraveled = 0;

            //Creates a hurtbox and hitbox for this projectile
            this.hurtbox = new Circle(startingLocation, (float)(Math.Log(damage, 1.08)));
            this.hitbox = new Rect(startingLocation, HITBOX_SIZE, HITBOX_SIZE);

            //Sets proper position for projectile, relative to center of sender
            hurtbox.SetCenter(startingLocation);
            hitbox.SetCenter(startingLocation);

            this.damage = damage;

            this.colour = colour;

            this.type = type;
        }

        /// <summary>
        /// Updates the position of the projectile
        /// </summary>
        /// <param name="particleField">A valid reference List of Particles</param>
        public void Update(ref List<Particle> particleField)
        {
            //The distance on the x and y axes that the projectile must move
            float xMove = (float)(Math.Cos(angle) * speed);
            float yMove = (float)(Math.Sin(angle) * speed);

            //Updates the location of the hurtbox and hitbox
            hurtbox.SetLocation(hurtbox.GetLocation() + new Vector2(xMove, yMove));
            hitbox.SetCenter(hurtbox.GetCenter());

            //Accounts for the newly-traveled distance
            distanceTraveled += speed;

            //Adding 2 particles to the screen
            for (int i = 0; i < 2; i++)
            {
                //Generates an angle for the projectile
                double partAngle = rng.Next(0, (int)(600 * Math.PI)) / 100;

                //Sets the particle's speed to a fraction of the projectile's speed
                double partSpeed = 0.5;

                //Sets the particle's rate of slowing or accelleration
                double partResistance = rng.Next(1, 121) / 100;

                //The particle will either fly straight, or turn clockwisecounterclockwise as it flies
                double partSpiralRate = rng.Next(-11, 11) / 100;

                //Deviation from the centre of the projectile
                Vector2 deviation = new Vector2(rng.Next(-30, 30), rng.Next(-30, 30));

                //Adds the particle to the particleField
                particleField.Add(new Particle(hurtbox.GetCenter() + deviation, colour, partAngle, partSpeed, partResistance, partSpiralRate, 500));
            }
        }

        /// <summary>
        /// The angle this projectile is flying at
        /// </summary>
        /// <returns>Angle, as a double</returns>
        public double GetAngle()
        {
            return angle;
        }

        /// <summary>
        /// The speed of this projectile, in pixels per update
        /// </summary>
        /// <returns>Speed, as a double</returns>
        public double GetSpeed()
        {
            return speed;
        }

        /// <summary>
        /// The hurtbox of this projectile
        /// </summary>
        /// <returns>A Circle Shape</returns>
        public Shape GetHurtbox()
        {
            return hurtbox;
        }

        /// <summary>
        /// The hitbox of this projectile
        /// </summary>
        /// <returns>A Rect Shape</returns>
        public Shape GetHitbox()
        {
            return hitbox;
        }

        /// <summary>
        /// The damage of this projectile
        /// </summary>
        /// <returns>Damage, as a double</returns>
        public double GetDamage()
        {
            return damage;
        }

        /// <summary>
        /// The Color that this projectile is to be drawn as
        /// </summary>
        /// <returns>A Color</returns>
        public Color GetColour()
        {
            return colour;
        }

        /// <summary>
        /// The type of this projectile
        /// </summary>
        /// <returns>Type, as a string</returns>
        public string GetProjectileType()
        {
            return type;
        }

        /// <summary>
        /// Sets a new angle for the projectile to fly at
        /// </summary>
        /// <param name="newAngle">A valid double</param>
        public void SetAngle(double newAngle)
        {
            angle = newAngle;
        }

        /// <summary>
        /// Sets a new speed for the projectile to fly at
        /// </summary>
        /// <param name="newSpeed">A valid double</param>
        public void SetSpeed(double newSpeed)
        {
            speed = newSpeed;
        }

        /// <summary>
        /// Indicates if this projectile has run out of range
        /// </summary>
        /// <returns>TRUE if distance traveled is grater than max range, otherwise FALSE</returns>
        public bool IsOutOfRange()
        {
            return (distanceTraveled >= range);
        }
    }
}
