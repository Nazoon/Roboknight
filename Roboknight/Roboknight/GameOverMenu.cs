//Author: Ilan Segal
//File Name: GameOverMenu.cs
//Project Name: RoboKnight
//Creation Date: January 19, 2016
//Modified Date: January 19, 2016
//Description: Class which represents the "Game Over" screen.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Roboknight
{
    class GameOverMenu
    {
        //Model state
        private ModelState modelState;

        //Button colours
        private Color defaultRetryColour;
        private Color defaultMainMenuColour;
        private Color defaultQuitColour;
        private Color hoverRetryColour;
        private Color hoverMainMenuColour;
        private Color hoverQuitColour;

        //Action buttons
        private Button retryButton;
        private Button mainMenuButton;
        private Button quitButton;

        //Constants used for creating buttons
        private const int BUTTON_WIDTH = 400;
        private const int BUTTON_HEIGHT = 100;

        /// <summary>
        /// Constructor for the GameOverMenu class
        /// </summary>
        public GameOverMenu()
        {
            //Defaults state to GameOver
            modelState = ModelState.GameOver;

            //Setting up button colours
            defaultRetryColour = Color.Goldenrod;
            defaultMainMenuColour = Color.DarkRed;
            defaultQuitColour = Color.Purple;
            hoverRetryColour = Color.LightGoldenrodYellow;
            hoverMainMenuColour = Color.Pink;
            hoverQuitColour = Color.MediumPurple;

            //Buttons
            retryButton = new Button(new Rectangle(600 - BUTTON_WIDTH / 2, 20, BUTTON_WIDTH, BUTTON_HEIGHT), "Retry!", defaultRetryColour, new Vector2(512, 40));
            mainMenuButton = new Button(new Rectangle(600 - BUTTON_WIDTH / 2, 20 + BUTTON_HEIGHT, BUTTON_WIDTH, BUTTON_HEIGHT), "Main Menu", defaultRetryColour, new Vector2(450, 140));
            quitButton = new Button(new Rectangle(600 - BUTTON_WIDTH / 2, 20 + 2 * BUTTON_HEIGHT, BUTTON_WIDTH, BUTTON_HEIGHT), "Quit", defaultRetryColour, new Vector2(525, 240));
        }

        /// <summary>
        /// Updates the GameOverMenu
        /// </summary>
        /// <param name="ms">A valid MouseState</param>
        public void Update(MouseState ms)
        {
            //Updates the retry button
            if (retryButton.Hover(ms) == true)
            {
                //Hover colour
                retryButton.SetColour(hoverRetryColour);

                //Button has been clicked, perform action
                if (retryButton.WasClicked(ms) == true)
                {
                    modelState = ModelState.Game;
                }
            }
            else
            {
                //Default colour
                retryButton.SetColour(defaultRetryColour);
            }

            //Updates the main menu button
            if (mainMenuButton.Hover(ms) == true)
            {
                //Hover colour
                mainMenuButton.SetColour(hoverMainMenuColour);

                //Button has been clicked, perform action
                if (mainMenuButton.WasClicked(ms) == true)
                {
                    modelState = ModelState.MainMenu;
                }
            }
            else
            {
                //Default colour
                mainMenuButton.SetColour(defaultMainMenuColour);
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
        /// Gets a list of all buttons in this menu
        /// </summary>
        /// <returns>A List of Buttons</returns>
        public List<Button> GetAllButtons()
        {
            return new List<Button> { retryButton, mainMenuButton, quitButton };
        }

        /// <summary>
        /// Sets the state of this GameOverMenu
        /// </summary>
        /// <param name="state">A valid ModelState</param>
        public void SetState(ModelState state)
        {
            modelState = state;
        }
    }
}
