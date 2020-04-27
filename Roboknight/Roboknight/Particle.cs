//Author: Ilan Segal
//File Name: Particle.cs
//Project Name: RoboKnight
//Creation Date: January 13, 2016
//Modified Date: January 13, 2016
//Description: Represents a particle object.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class Particle
    {
        //Location of the partilce
        private Vector2 location;

        //Colour of the particle
        private Color colour;

        //The particle's trajectory information
        private double angle;
        private double speed;
        private double resistance;
        private double spiralRate;

        //Lifespan information
        private int lifespan;
        private int maxLifespan;

        /// <summary>
        /// Constructor for the Particle class
        /// </summary>
        /// <param name="location">A valid Vector2</param>
        /// <param name="colour">A valid Color</param>
        /// <param name="angle">The angle at which this particle will fly</param>
        /// <param name="speed">The pixels per update that this particle will move</param>
        /// <param name="resistance">The rate at which this particle will lose speed</param>
        /// <param name="spiralRate">The rate at which this particle will turn counterclockwise</param>
        /// <param name="lifespan">The milliseconds that this particle will last for</param>
        public Particle(Vector2 location, Color colour, double angle, double speed, double resistance, double spiralRate, int lifespan)
        {
            this.location = location;

            this.colour = colour;

            this.angle = angle;
            this.speed = speed;
            this.resistance = resistance;
            this.spiralRate = spiralRate;

            this.lifespan = 0;
            this.maxLifespan = lifespan;
        }

        /// <summary>
        /// Updates this Particle
        /// </summary>
        /// <param name="gameTime">A valid GameTime</param>
        public void Update(GameTime gameTime)
        {
            location.X += (float)(Math.Cos(angle) * speed);
            location.Y += (float)(Math.Sin(angle) * speed);

            speed /= resistance;
            angle += spiralRate;

            lifespan += gameTime.ElapsedGameTime.Milliseconds;
        }

        /// <summary>
        /// Gets this Particles location
        /// </summary>
        /// <returns>A Vector2 object</returns>
        public Vector2 GetLocation()
        {
            return location;
        }

        /// <summary>
        /// Gets this Particles colour
        /// </summary>
        /// <returns>A Colour object</returns>
        public Color GetColour()
        {
            return colour;
        }

        /// <summary>
        /// Will tell if this Particle has expended its lifespan
        /// </summary>
        /// <returns>TRUE if this Particle has expended its lifespan, otherwise FALSE</returns>
        public bool IsOutOfRange()
        {
            return (lifespan > maxLifespan);
        }
    }
}
