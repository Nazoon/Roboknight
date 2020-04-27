//Author: Ilan Segal
//File Name: Button.cs
//Project Name: Tetris
//Date Created: January 9, 2016
//Modified Date: January 12, 2016
//Desc: Data structure allows for buttons on a screen

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roboknight
{
    class Button
    {
        //Data dealing with the drawing of the button
        private Rectangle box;
        private string text;
        private Color colour;
        private Vector2 textPos;

        /// <summary>
        /// Constructor for the Button class
        /// </summary>
        /// <param name="box">A valid Rectangle</param>
        /// <param name="text">The text to be displayed on the button</param>
        /// <param name="colour">A valid Color</param>
        /// <param name="textPos">A valid Vector2</param>
        public Button(Rectangle box, string text, Color colour, Vector2 textPos)
        {
            this.box = box;
            this.text = text;
            this.colour = colour;
            this.textPos = textPos;
        }

        /// <summary>
        /// Allows the program to check if this button was clicked
        /// </summary>
        /// <param name="ms">A valid MouseState</param>
        /// <returns>TRUE only if the cursor click was within the bounds of this Button</returns>
        public bool WasClicked(MouseState ms)
        {
            return (ms.LeftButton == ButtonState.Pressed && this.withinClickBox(ms.X, ms.Y));
        }

        /// <summary>
        /// Allows the program to detect a cursor hovering over this button
        /// </summary>
        /// <param name="ms">A valid MouseState</param>
        /// <returns>TRUE only if the cursor is within the bounds of this Button</returns>
        public bool Hover(MouseState ms)
        {
            return this.withinClickBox(ms.X, ms.Y);
        }

        /// <summary>
        /// Gets the box of this Button
        /// </summary>
        /// <returns>A Rectangle object</returns>
        public Rectangle GetBox()
        {
            return box;
        }

        /// <summary>
        /// Gets the text on this Button
        /// </summary>
        /// <returns>A string primitive</returns>
        public string GetText()
        {
            return text;
        }

        /// <summary>
        /// Gets the colour of this Button
        /// </summary>
        /// <returns>A Color object</returns>
        public Color GetColour()
        {
            return colour;
        }

        /// <summary>
        /// Gets the position of the text on this Button
        /// </summary>
        /// <returns>A Vector2 object</returns>
        public Vector2 GetTextPos()
        {
            return textPos;
        }

        /// <summary>
        /// Sets the text on the button
        /// </summary>
        /// <param name="newText">A valid string</param>
        public void SetText(string newText)
        {
            text = newText;
        }

        /// <summary>
        /// Sets this Button's colour
        /// </summary>
        /// <param name="newColor">A valid Color</param>
        public void SetColour(Color newColor)
        {
            colour = newColor;
        }

        /// <summary>
        /// Allows this button to check if a click was inside the bounds of this button
        /// NOTE: Internal use only
        /// </summary>
        /// <param name="x">X-Position of cursor</param>
        /// <param name="y">Y-position of cursor</param>
        /// <returns>TRUE only if the cursor click was within the bounds of this Button</returns>
        private bool withinClickBox(int x, int y)
        {
            //Checks bounds
            if (x < box.X || x > box.X + box.Width)
            {
                return false;
            }
            if (y < box.Y || y > box.Y + box.Height)
            {
                return false;
            }

            //None of the fail conditions have been met, return TRUE
            return true;
        }
    }
}