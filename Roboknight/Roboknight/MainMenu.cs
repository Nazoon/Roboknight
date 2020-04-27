//Author: Ilan Segal
//File Name: MainMenu.cs
//Project Name: RoboKnight
//Creation Date: January 18, 2016
//Modified Date: January 19, 2016
//Description: Class which represents the main menu screen.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Roboknight
{
    class MainMenu
    {
        //Model state
        private ModelState modelState;

        //Button colours
        private Color defaultPlayColour;
        private Color defaultQuitColour;
        private Color hoverPlayColour;
        private Color hoverQuitColour;

        //Action buttons
        private Button playButton;
        private Button quitButton;

        //Constants used for creating buttons
        private const int BUTTON_WIDTH = 400;
        private const int BUTTON_HEIGHT = 100;

        /// <summary>
        /// Constructor for the MainMenu class
        /// </summary>
        public MainMenu()
        {
            //Setting up the state for the main model to follow
            modelState = ModelState.MainMenu;

            //Setting up colours
            defaultPlayColour = Color.Goldenrod;
            defaultQuitColour = Color.Purple;
            hoverPlayColour = Color.LightGoldenrodYellow;
            hoverQuitColour = Color.MediumPurple;

            //Setting up buttons
            playButton = new Button(new Rectangle(1200 / 2 - BUTTON_WIDTH / 2, 675 / 2, BUTTON_WIDTH, BUTTON_HEIGHT), "Play", defaultPlayColour, new Vector2(140 + 1200 / 2 - BUTTON_WIDTH / 2, 20 + 675 / 2));
            quitButton = new Button(new Rectangle(1200 / 2 - BUTTON_WIDTH / 2, 675 / 2 + BUTTON_HEIGHT, BUTTON_WIDTH, BUTTON_HEIGHT), "Quit", defaultQuitColour, new Vector2(132 + 1200 / 2 - BUTTON_WIDTH / 2, 20 + 675 / 2 + BUTTON_HEIGHT));
        }

        /// <summary>
        /// Updates the main menu
        /// </summary>
        /// <param name="ms">A valid MouseState</param>
        public void Update(MouseState ms)
        {
            //Updates the play button
            if (playButton.Hover(ms) == true)
            {
                //Hover colour
                playButton.SetColour(hoverPlayColour);

                //Button has been clicked, perform action
                if (playButton.WasClicked(ms) == true)
                {
                    modelState = ModelState.Game;
                }
            }
            else
            {
                //Default colour
                playButton.SetColour(defaultPlayColour);
            }

            //Updates the quit button
            if (quitButton.Hover(ms) == true)
            {
                //Hover colour
                quitButton.SetColour(hoverQuitColour);

                if (quitButton.WasClicked(ms))
                {
                    //Button has been clicked, perform action
                    modelState = ModelState.Quit;
                }
            }
            else
            {
                //Default colour
                quitButton.SetColour(defaultQuitColour);
            }
        }

        /// <summary>
        /// Gets the state that the Model should be in
        /// </summary>
        /// <returns>A ModelState object</returns>
        public ModelState GetState()
        {
            return modelState;
        }

        /// <summary>
        /// Gets a list of all buttons on this menu
        /// </summary>
        /// <returns>A List of Buttons</returns>
        public List<Button> GetAllButtons()
        {
            return new List<Button> { playButton, quitButton };
        }

        /// <summary>
        /// Sets the state of this menu
        /// </summary>
        /// <param name="state">A valid ModelState</param>
        public void SetState(ModelState state)
        {
            modelState = state;
        }
    }
}
