//Author: Ilan Segal
//File Name: Driver.cs
//Project Name: RoboKnight
//Creation Date: December 7, 2015
//Modified Date: January 15, 2015
//Description: Hosts the interactions between the Model, View, and Controller classes. Drives the game.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Roboknight
{
    public class Driver : Microsoft.Xna.Framework.Game
    {
        //Graphics data
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //The game
        Model model;
        KeyboardController kbController;
        GamePadController gpController;
        View view;

        /// <summary>
        /// Construction for the Driver class
        /// </summary>
        public Driver()
        {
            this.IsMouseVisible = true;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Setting up screen size and attributes
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 675;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {            
            model = new Model(Content);
            kbController = new KeyboardController(ref model);
            gpController = new GamePadController(ref model);
            view = new View(Content);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Updates game
            if (GamePad.GetState(PlayerIndex.One).IsConnected == true)
            {
                gpController.Update();

                //Tricks the model into operating with the cursor simulated by the GamePadController
                model.Update(gameTime, gpController.GetSimMouseState());
            }
            else
            {
                kbController.Update();
                model.Update(gameTime, Mouse.GetState());
            }

            //Exits the game if the quit button was pressed
            if (model.GetState() == ModelState.Quit)
            {
                this.Exit();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Clears screen
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Draws game to the spritebatch
            view.Draw(spriteBatch, model);

            //Draws the spritebatch to the screen
            base.Draw(gameTime);
        }
    }
}
