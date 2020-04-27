//Author: Ilan Segal
//File Name: GamepadController.cs
//Project Name: RoboKnight
//Creation Date: January 18, 2015
//Modified Date: January 18, 2016
//Description: The class which manages how the game's Model behaves (takes input from XBox 360 Gamepad).

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roboknight
{
    class GamePadController
    {
        //Data used to control the game's Model
        private Model refModel;
        private GamePadState curState;
        private GamePadState prevState;
        private MouseState simMouseState;

        private const float MOUSE_MOVEMENT_RATE = 10;

        /// <summary>
        /// The constructor for the GamePadController class
        /// </summary>
        /// <param name="refModel"></param>
        public GamePadController(ref Model refModel)
        {
            this.refModel = refModel;

            curState = GamePad.GetState(PlayerIndex.One);
            prevState = GamePad.GetState(PlayerIndex.One);

            simMouseState = new MouseState();
        }

        /// <summary>
        /// Updates the GamepadController
        /// </summary>
        public void Update()
        {
            //Updates curstate
            curState = GamePad.GetState(PlayerIndex.One);

            //Updates directional controls
            this.MoveStateUpdate();
            this.WeaponStateUpdate();

            //Moving between menus
            if (curState != prevState)
            {
                if (curState.Buttons.Start == ButtonState.Pressed && prevState.Buttons.Start != ButtonState.Pressed)
                {
                    switch (refModel.GetState())
                    {
                        case ModelState.MainMenu:

                            refModel.SetState(ModelState.Game);
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

                //Updates prevState
                prevState = curState;
            }

            //Moving the cursor while in the pause menu or main menu
            if (refModel.GetState() == ModelState.PauseMenu || refModel.GetState() == ModelState.MainMenu)
            {
                Mouse.SetPosition((int)(Mouse.GetState().X + MOUSE_MOVEMENT_RATE * curState.ThumbSticks.Left.X), (int)(Mouse.GetState().Y + MOUSE_MOVEMENT_RATE * -curState.ThumbSticks.Left.Y));

                ButtonState leftButton = ButtonState.Released;
                if (curState.Buttons.A == ButtonState.Pressed)
                {
                    leftButton = ButtonState.Pressed;
                }

                //Simulates cursor
                simMouseState = new MouseState((int)(Mouse.GetState().X), (int)(Mouse.GetState().Y), 0, leftButton, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
            }
        }

        /// <summary>
        /// Gets the simulated MouseState
        /// </summary>
        /// <returns></returns>
        public MouseState GetSimMouseState()
        {
            return simMouseState;
        }

        /// <summary>
        /// Updates this GamepadController's control over the Model's MoveState
        /// </summary>
        private void MoveStateUpdate()
        {
            refModel.SetMoveVector(new Vector2(curState.ThumbSticks.Left.X, -curState.ThumbSticks.Left.Y));
        }

        /// <summary>
        /// Updates this GamepadController's control over the Model's WeaponState
        /// </summary>
        private void WeaponStateUpdate()
        {
            if (curState.Buttons.B == ButtonState.Pressed)
            {
                refModel.SetWeaponState(WeaponState.Right);
            }
            else if (curState.Buttons.X == ButtonState.Pressed)
            {
                refModel.SetWeaponState(WeaponState.Left);
            }
            else if (curState.Buttons.Y == ButtonState.Pressed)
            {
                refModel.SetWeaponState(WeaponState.Up);
            }
            else if (curState.Buttons.A  == ButtonState.Pressed)
            {
                refModel.SetWeaponState(WeaponState.Down);
            }
            else
            {
                refModel.SetWeaponState(WeaponState.None);
            }
        }
    }
}
