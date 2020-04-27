//Author: Ilan Segal
//File Name: KeyboardsController.cs
//Project Name: RoboKnight
//Creation Date: December 5, 2015
//Modified Date: January 9, 2016
//Description: The class which manages how the game's Model behaves (takes input from keyboard).

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roboknight
{
    class KeyboardController
    {
        //Data used to control the game's Model
        private Model refModel;
        private KeyboardState prevState;

        //Speed percent to move player on x and y axes, depending on direction
        const float SPEED_PERCENT = 1;
        const float SPEED_PERCENT_DIAGONAL = 0.71F;

        /// <summary>
        /// Constructor for the KeyboardController class
        /// </summary>
        /// <param name="model">A valid Model to control</param>
        public KeyboardController(ref Model model)
        {
            refModel = model;

            prevState = Keyboard.GetState();
        }

        /// <summary>
        /// Updates the move/weapon state of this Controller's target Model
        /// </summary>
        public void Update()
        {
            //Updates the player's movement vector
            this.MoveStateUpdate();

            //Fires a projectile if the user wants to
            this.WeaponStateUpdate();

            //Other key presses
            if (!Keyboard.GetState().Equals(prevState))
            {
                //Functions of the Esc key
                if (Keyboard.GetState().IsKeyDown(Keys.Escape) == true)
                {
                    switch (refModel.GetState())
                    {
                        case ModelState.MainMenu:

                            refModel.SetState(ModelState.Quit);
                            break;

                        case ModelState.Game:

                            refModel.GetPauseMenu().UpdateMap();
                            refModel.SetState(ModelState.PauseMenu);
                            break;

                        case ModelState.PauseMenu:

                            refModel.SetState(ModelState.Game);
                            break;
                    }
                }

                //Prevents multiple unintentional inputs
                prevState = Keyboard.GetState();
            }
        }

        /// <summary>
        /// Updates the refModel's moveState
        /// </summary>
        private void MoveStateUpdate()
        {
            if ((Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.S)))
            {
                //The W and S keys cancel out, so check for other movement input
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    //Sets model to move player LEFT
                    refModel.SetMoveVector(new Vector2(-SPEED_PERCENT, 0));
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    //Sets model to move player RIGHT
                    refModel.SetMoveVector(new Vector2(SPEED_PERCENT, 0));
                }
                else
                {
                    //The inputted keys canel out, so don't move
                    refModel.SetMoveVector(Vector2.Zero);
                }
            }
            else if ((Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.D)))
            {
                //The A and D keys cancel out, so check for other movement input
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    //Sets model to move player UP
                    refModel.SetMoveVector(new Vector2(0, -SPEED_PERCENT));
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    //Sets model to move player DOWN
                    refModel.SetMoveVector(new Vector2(0, SPEED_PERCENT));
                }
                else
                {
                    //The inputted keys canel out, so don't move
                    refModel.SetMoveVector(Vector2.Zero);
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.A))
            {
                //Sets model to move player UP and LEFT
                refModel.SetMoveVector(new Vector2(-SPEED_PERCENT_DIAGONAL, -SPEED_PERCENT_DIAGONAL));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.S))
            {
                //Sets model to move player DOWN and LEFT
                refModel.SetMoveVector(new Vector2(-SPEED_PERCENT_DIAGONAL, SPEED_PERCENT_DIAGONAL));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.D))
            {
                //Sets model to move player DOWN and RIGHT
                refModel.SetMoveVector(new Vector2(SPEED_PERCENT_DIAGONAL, SPEED_PERCENT_DIAGONAL));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D) && Keyboard.GetState().IsKeyDown(Keys.W))
            {
                //Sets model to move player UP and RIGHT
                refModel.SetMoveVector(new Vector2(SPEED_PERCENT_DIAGONAL, -SPEED_PERCENT_DIAGONAL));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                //Sets model to move player UP
                refModel.SetMoveVector(new Vector2(0, -SPEED_PERCENT));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                //Sets model to move player UP
                refModel.SetMoveVector(new Vector2(-SPEED_PERCENT, 0));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                //Sets model to move player DOWN
                refModel.SetMoveVector(new Vector2(0, SPEED_PERCENT));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                //Sets model to move player RIGHT
                refModel.SetMoveVector(new Vector2(SPEED_PERCENT, 0));
            }
            else
            {
                //Model will not move player
                refModel.SetMoveVector(Vector2.Zero);
            }
        }

        /// <summary>
        /// Updates the refModel's weaponState
        /// </summary>
        private void WeaponStateUpdate()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                //Sets the model to fire a projectile UP
                refModel.SetWeaponState(WeaponState.Up);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                //Sets the model to fire a projectile DOWN
                refModel.SetWeaponState(WeaponState.Down);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                //Sets the model to fire a projectile LEFT
                refModel.SetWeaponState(WeaponState.Left);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                //Sets the model to fire a projectile RIGHT
                refModel.SetWeaponState(WeaponState.Right);
            }
            else
            {
                //Model will not fire any projectile
                refModel.SetWeaponState(WeaponState.None);
            }
        }
    }
}
