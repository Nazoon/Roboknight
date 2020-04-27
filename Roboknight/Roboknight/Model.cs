//Author: Ilan Segal
//File Name: Model.cs
//Project Name: RoboKnight
//Creation Date: December 5, 2015
//Modified Date: January 19, 2016
//Description: Allows the GameField class to update and behave properly. Allows for interaction of GameField with Controller and View classes.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Roboknight
{
    class Model
    {
        //Content manager
        private ContentManager content;

        //Used to prevent any kind of double-input
        private MouseState prevMS;

        //The game mechanics
        private GameField game;

        //State variables, controlling how the GameField behaves
        private Vector2 moveVector;
        private WeaponState weaponDirection;

        //State of the model
        private ModelState modelState;

        //Menus
        private MainMenu mainMenu;
        private PauseMenu pauseMenu;
        private GameOverMenu gameOverMenu;

        //Background music data
        private SoundEffect titleMusic;
        private SoundEffect dungeonMusic;
        private SoundEffect gameOverMusic;
        private SoundEffectInstance titleInstance;
        private SoundEffectInstance dungeonInstance;
        private SoundEffectInstance gameOverInstance;

        //Sound effects
        private SoundEffect pauseEffect;
        
        /// <summary>
        /// Constructor for the Model class
        /// </summary>
        /// <param name="content">A valid ContentManager</param>
        public Model(ContentManager content)
        {
            //Setting up content manager
            this.content = content;

            //Setting up Prev MS
            prevMS = Mouse.GetState();

            //Starts game, with a maximum floor size (length and width) of 100 rooms
            game = new GameField(content, 100);

            //Starts both states as neutral
            moveVector = Vector2.Zero;
            weaponDirection = WeaponState.None;

            //The model begins with the game in focus
            modelState = ModelState.MainMenu;

            //Instantiates the menus
            mainMenu = new MainMenu();
            PlayerStats playerStats = game.GetPlayer().GetStats();
            pauseMenu = new PauseMenu(this.content, game.GetCurrentRoom(), ref playerStats);
            gameOverMenu = new GameOverMenu();

            //Setting up music
            titleMusic = this.content.Load<SoundEffect>(@"Sound\Music\title");
            dungeonMusic = this.content.Load<SoundEffect>(@"Sound\Music\dungeon");
            gameOverMusic = this.content.Load<SoundEffect>(@"Sound\Music\gameover");

            //Setting up effects
            pauseEffect = this.content.Load<SoundEffect>(@"Sound\Menu\pause");

            //Setting up music instances
            titleInstance = titleMusic.CreateInstance();
            dungeonInstance = dungeonMusic.CreateInstance();
            gameOverInstance = gameOverMusic.CreateInstance();
            titleInstance.IsLooped = true;
            dungeonInstance.IsLooped = true;
            gameOverInstance.IsLooped = false; //Game-over music sounds best when not looped
            titleInstance.Volume = 0.3F;
            dungeonInstance.Volume = 0.2F;
            gameOverInstance.Volume = 0.3F;

            //Starts off with the main menu music
            titleInstance.Play();
        }

        /// <summary>
        /// Updates the game
        /// </summary>
        /// <param name="gameTime">A valid GameTime</param>
        /// <param name="ms">A valid MouseState</param>
        public void Update(GameTime gameTime, MouseState ms)
        {
            switch(modelState)
            {
                case ModelState.Game:

                    game.Update(gameTime, moveVector, weaponDirection);

                    //Checks for Game-over condition
                    if (game.GetPlayer().GetStats().GetHealth() <= 0)
                    {
                        modelState = ModelState.GameOver;

                        //Plays game over music
                        dungeonInstance.Stop();
                        gameOverInstance.Play();
                    }

                    break;

                case ModelState.MainMenu:
                                        
                    mainMenu.Update(ms);
                                        
                    modelState = mainMenu.GetState();

                    if (modelState == ModelState.Game)
                    {
                        //Resets game
                        game = new GameField(content, 100);
                        PlayerStats playerStats = game.GetPlayer().GetStats();
                        pauseMenu = new PauseMenu(this.content, game.GetCurrentRoom(), ref playerStats);
                        gameOverMenu = new GameOverMenu();

                        //Resets main menu state
                        mainMenu.SetState(ModelState.MainMenu);

                        //Plays game music
                        titleInstance.Stop();
                        dungeonInstance.Play();
                    }

                    break;
                    
                case ModelState.PauseMenu:

                    pauseMenu.Update(ms, game.GetCurrentRoom());

                    break;

                case ModelState.GameOver:

                    gameOverMenu.Update(ms);                                        
                    modelState = gameOverMenu.GetState();

                    //Resets the game if the player selects 
                    if (modelState == ModelState.Game)
                    {
                        //Resets game
                        game = new GameField(content, 100);

                        //Resets game over screen
                        gameOverMenu.SetState(ModelState.GameOver);
                       
                        //Plays game music
                        gameOverInstance.Stop();
                        dungeonInstance.Play();
                    }
                    else if (modelState == ModelState.MainMenu)
                    {
                        //Plays main menu music
                        gameOverInstance.Stop();
                        titleInstance.Play();
                    }

                    break;
            }

            //Cheat codes!
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                //LShift + X: Player gets 1,000,000 G
                if (Keyboard.GetState().IsKeyDown(Keys.X))
                {
                    game.GetPlayer().GetStats().SetGold(1000000);
                }

                //LShift + C: Player gets 1,000,000 HP
                if (Keyboard.GetState().IsKeyDown(Keys.C))
                {
                    game.GetPlayer().GetStats().SetHealth(1000000);
                }

                //LShift + Space: Clears all enemies in the current room
                //NOTE: Player does not recieve money for clearing room
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    game.GetCurrentRoom().GetEnemies().Clear();
                }

                //LShift + E: Clears all obstacles in the current room
                //NOTE: Will clear walls too, use at your own risk (you can walk into rooms that don't exist and crash the game)
                if (Keyboard.GetState().IsKeyDown(Keys.E))
                {
                    game.GetCurrentRoom().GetObstacles().Clear();
                }
            }
        }

        /// <summary>
        /// Gets the angle that the player is to move in
        /// </summary>
        /// <returns>A double value</returns>
        public double GetMoveAngle()
        {
            return Math.Atan(moveVector.Y / moveVector.X);
        }

        /// <summary>
        /// Gets the current direction that the player is to shoot at
        /// </summary>
        /// <returns>A WeaponState value</returns>
        public WeaponState GetWeaponState()
        {
            return weaponDirection;
        }

        /// <summary>
        /// Gets the current state of the model
        /// </summary>
        /// <returns>A ModelState value</returns>
        public ModelState GetState()
        {
            return modelState;
        }

        /// <summary>
        /// The game's player
        /// </summary>
        /// <returns>The stored GameField's PlayerCharacter</returns>
        public PlayerCharacter GetPlayerCharacter()
        {
            return game.GetPlayer();
        }

        /// <summary>
        /// The basic game
        /// </summary>
        /// <returns>A GameField object</returns>
        public GameField GetGame()
        {
            return game;
        }

        /// <summary>
        /// Gets the main menu
        /// </summary>
        /// <returns>A MainMenu object</returns>
        public MainMenu GetMainMenu()
        {
            return mainMenu;
        }

        /// <summary>
        /// Gets the pause menu
        /// </summary>
        /// <returns>A PauseMenu object</returns>
        public PauseMenu GetPauseMenu()
        {
            return pauseMenu;
        }

        /// <summary>
        /// Gets the "Game Over" menu
        /// </summary>
        /// <returns>A GasmeOverMenu object</returns>
        public GameOverMenu GetGameOverMenu()
        {
            return gameOverMenu;
        }

        /// <summary>
        /// Sets the direction for the player to move upon the next update
        /// </summary>
        /// <param name="moveVector">A valid Vector2</param>
        public void SetMoveVector(Vector2 moveVector)
        {
            this.moveVector = moveVector;
        }

        /// <summary>
        /// Sets the direction for the player to fire at upon the next update
        /// </summary>
        /// <param name="weaponDirection">A valid WeaponState</param>
        public void SetWeaponState(WeaponState weaponDirection)
        {
            this.weaponDirection = weaponDirection;
        }

        /// <summary>
        /// Sets the model's state
        /// </summary>
        /// <param name="newModelState">A valid ModelState</param>
        public void SetState(ModelState newModelState)
        {
            //Pause menu sound effect
            if ((modelState == ModelState.Game && newModelState == ModelState.PauseMenu) || (modelState == ModelState.PauseMenu && newModelState == ModelState.Game))
            {
                pauseEffect.Play();
            }

            modelState = newModelState;
        }
    }
}
