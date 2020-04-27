//Author: Ilan Segal
//File Name: View.cs
//Project Name: RoboKnight
//Creation Date: December 5, 2015
//Modified Date: April 15, 2015
//Description: Draws the game (Model) to the screen.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class View
    {
        //Menu backgrounds
        private Texture2D mainMenuBackground;
        private Texture2D gameOverBackground;
        private Texture2D pauseMenuBackground;

        //Environment Textures
        private Texture2D floorTexture;
        private Texture2D wallTexture;
        private Texture2D rockTexture;

        //Item sprites
        private Texture2D pedestalTexture;
        private Texture2D batteryTexture;
        private Texture2D nuclearTexture;
        private Texture2D chessTexture;
        private Texture2D faradayTexture;
        private Texture2D scopeTexture;
        private Texture2D aluminumTexture;
        private Texture2D coolingTexture;
        private Texture2D lensTexture;
        private Texture2D crystalTexture;
        private Texture2D grapheneTexture;
        private Texture2D nanoTexture;
        private Texture2D onionTexture;
        private Texture2D overclockedTexture;
        private Texture2D titanXTexture;
        private Texture2D skynetTexture;
        private Texture2D quadCoreTexture;

        //Menu textures/sprites
        private Texture2D buttonTexture;

        //Character Sprites
        private Texture2D playerFrontTexture;
        private Texture2D playerLeftTexture;
        private Texture2D playerRightTexture;
        private Texture2D playerBackTexture;
        private Texture2D nanocloudTexture;
        private Texture2D turretTexture;
        private Texture2D weepingTurretTexture;
        private Texture2D broodingTurretTexture;
        private Texture2D keladTexture;
        private Texture2D sandmanTexture;
        private Texture2D sandpileTexture;
        private Texture2D projectileTexture;

        //Fonts
        private SpriteFont headerFont;
        private SpriteFont buttonFont;
        private SpriteFont descFont;
        
        //Used to draw map
        private const int MAP_ICON_SIZE = 40;
        private const int MAP_ICON_X_OFFSET = 100;
        private const int MAP_ICON_Y_OFFSET = 100;
        private ContentManager contentManager;

        //Used when rounding the player's movement angle
        private const double ANGLE_RIGHT = 0;
        private const double ANGLE_UPRIGHT = Math.PI * 0.25;
        private const double ANGLE_UP = Math.PI * 0.5;
        private const double ANGLE_UPLEFT = Math.PI * 0.75;
        private const double ANGLE_LEFT = Math.PI * 1;
        private const double ANGLE_DOWNLEFT = Math.PI * 1.25;
        private const double ANGLE_DOWN = Math.PI * 1.5;
        private const double ANGLE_DOWNRIGHT = Math.PI * 1.75;
        

        //Size of particles when drawn to the screen
        private const int PARTICLE_SIZE = 5;

        /// <summary>
        /// Constructor for the View class
        /// </summary>
        /// <param name="content">A valid ContentManager</param>
        public View(ContentManager content)
        {
            //Loading up menu backgrounds
            mainMenuBackground = content.Load<Texture2D>(@"Images\Menu Backgrounds\titlescreen");
            gameOverBackground = content.Load<Texture2D>(@"Images\Menu Backgrounds\gameoverscreen");
            pauseMenuBackground = content.Load<Texture2D>(@"Images\Menu Backgrounds\pausescreen");

            //Loading up environment textures
            floorTexture = content.Load<Texture2D>(@"Images\Environment\floortexture");
            wallTexture = content.Load<Texture2D>(@"Images\Environment\walltexture");
            rockTexture = content.Load<Texture2D>(@"Images\Environment\rocktexture");

            //Loading up item sprites
            pedestalTexture = content.Load<Texture2D>(@"Images\Sprites\Items\pedestal");
            batteryTexture = content.Load<Texture2D>(@"Images\Sprites\Items\battery");
            nuclearTexture = content.Load<Texture2D>(@"Images\Sprites\Items\nuclear");
            chessTexture = content.Load<Texture2D>(@"Images\Sprites\Items\chess");
            faradayTexture = content.Load<Texture2D>(@"Images\Sprites\Items\faraday");
            scopeTexture = content.Load<Texture2D>(@"Images\Sprites\Items\silphscope");
            aluminumTexture = content.Load<Texture2D>(@"Images\Sprites\Items\aluminum");
            coolingTexture = content.Load<Texture2D>(@"Images\Sprites\Items\cooling");
            lensTexture = content.Load<Texture2D>(@"Images\Sprites\Items\lens");
            crystalTexture = content.Load<Texture2D>(@"Images\Sprites\Items\crystal");
            grapheneTexture = content.Load<Texture2D>(@"Images\Sprites\Items\graphene");
            nanoTexture = content.Load<Texture2D>(@"Images\Sprites\Items\nano");
            onionTexture = content.Load<Texture2D>(@"Images\Sprites\Items\onion");
            overclockedTexture = content.Load<Texture2D>(@"Images\Sprites\Items\overclocked");
            titanXTexture = content.Load<Texture2D>(@"Images\Sprites\Items\titanX");
            skynetTexture = content.Load<Texture2D>(@"Images\Sprites\Items\skynet");
            quadCoreTexture = content.Load<Texture2D>(@"Images\Sprites\Items\quadcore");

            //Loading up menu images
            buttonTexture = content.Load<Texture2D>("menu-button");

            //Loading up misc sprites
            playerFrontTexture = content.Load<Texture2D>(@"Images\Sprites\Roboknight\forward");
            playerLeftTexture = content.Load<Texture2D>(@"Images\Sprites\Roboknight\left");
            playerBackTexture = content.Load<Texture2D>(@"Images\Sprites\Roboknight\backward");
            playerRightTexture = content.Load<Texture2D>(@"Images\Sprites\Roboknight\right");
            projectileTexture = content.Load<Texture2D>(@"Images\Sprites\projectile");
            nanocloudTexture = content.Load<Texture2D>(@"Images\Sprites\Enemy\nanocloud");
            turretTexture = content.Load<Texture2D>(@"Images\Sprites\Enemy\turret");
            weepingTurretTexture = content.Load<Texture2D>(@"Images\Sprites\Enemy\weepingturret");
            broodingTurretTexture = content.Load<Texture2D>(@"Images\Sprites\Enemy\broodingturret");
            keladTexture = content.Load<Texture2D>(@"Images\Sprites\Enemy\kelad");
            sandmanTexture = content.Load<Texture2D>(@"Images\Sprites\Enemy\sandman");
            sandpileTexture = content.Load<Texture2D>(@"Images\Sprites\Enemy\sandpile");

            //Loading up fonts
            headerFont = content.Load<SpriteFont>(@"Fonts\HeaderFont");
            buttonFont = content.Load<SpriteFont>(@"Fonts\ButtonFont");
            descFont = content.Load<SpriteFont>(@"Fonts\DescFont");

            //Used strictly for map drawing, in which map icons are pulled from storage to save memory
            contentManager = content;
        }

        /// <summary>
        /// Draws all currently visible objects present in the given Model
        /// </summary>
        /// <param name="sb">A valid SpriteBatch</param>
        /// <param name="model">A valid Model (the game)</param>
        public void Draw(SpriteBatch sb, Model model)
        {
            //Opens the spritebatch
            //sb.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            sb.Begin();

            //Chooses what to draw, depending on the state of the Model
            switch(model.GetState())
            {
                //What to draw if the Model is currently updating the game
                case ModelState.Game:

                    //Draws floor
                    sb.Draw(floorTexture, new Rectangle(0, 0, 1200, 675), Color.DarkSeaGreen);

                    //Draws all items in the current room
                    foreach (ItemPedestal curItem in model.GetGame().GetCurrentRoom().GetItemPedestals())
                    {
                        //Gets the proper drawing dimensions and location of the current item pedestal
                        Rectangle pesestalRec = new Rectangle((int)curItem.GetHitbox().GetLocation().X, (int)curItem.GetHitbox().GetLocation().Y, (int)curItem.GetHitbox().GetSize(), (int)curItem.GetHitbox().GetSize());
                        Rectangle itemRec = new Rectangle((int)curItem.GetHitbox().GetLocation().X, (int)curItem.GetHitbox().GetLocation().Y - 50, (int)curItem.GetHitbox().GetSize(), (int)curItem.GetHitbox().GetSize());

                        //Draws the item pedestal
                        sb.Draw(pedestalTexture, pesestalRec, Color.Gray);

                        //Draws the item on the pedestal
                        this.DrawItem(sb, itemRec, curItem.GetItem());
                    }

                    //Draws all obstacles in the current room
                    foreach (Obstacle curObstacle in model.GetGame().GetCurrentRoom().GetObstacles())
                    {
                        //Gets the proper drawing dimensions and location of the current obstacle
                        Rectangle r = new Rectangle((int)curObstacle.GetHitbox().GetLocation().X, (int)curObstacle.GetHitbox().GetLocation().Y, (int)((Rect)(curObstacle.GetHitbox())).GetWidth(), (int)((Rect)(curObstacle.GetHitbox())).GetHeight());

                        //Decides what to draw, depending on the type of obstacle
                        switch (curObstacle.GetObstacleType())
                        {
                            case "rock":
                                sb.Draw(rockTexture, r, Color.DarkGreen);
                                break;

                            case "wall":
                                sb.Draw(wallTexture, r, Color.DarkGreen);
                                break;
                        }
                    }

                    //Draws all projectiles currently on screen
                    foreach (Projectile curProjectile in model.GetGame().GetProjectiles())
                    {
                        //Gets the proper drawing dimensions and location of the current projectile
                        Rectangle r = new Rectangle((int)curProjectile.GetHurtbox().GetLocation().X, (int)curProjectile.GetHurtbox().GetLocation().Y, (int)curProjectile.GetHurtbox().GetSize(), (int)curProjectile.GetHurtbox().GetSize());

                        //Draws the projectile
                        sb.Draw(projectileTexture, r, curProjectile.GetColour());
                    }

                    foreach (Particle curParticle in model.GetGame().GetParticles())
                    {
                        Rectangle r = new Rectangle((int)curParticle.GetLocation().X, (int)curParticle.GetLocation().Y, PARTICLE_SIZE, PARTICLE_SIZE);

                        sb.Draw(projectileTexture, r, curParticle.GetColour());
                    }

                    //Draws all enemies in the current room
                    foreach (Enemy curEnemy in model.GetGame().GetCurrentRoom().GetEnemies())
                    {
                        //Gets the proper drawing dimensions and location of the current enemy
                        Rectangle r = new Rectangle((int)curEnemy.GetObstacleHitbox().GetLocation().X, (int)curEnemy.GetObstacleHitbox().GetLocation().Y, (int)((Rect)(curEnemy.GetObstacleHitbox())).GetWidth(), (int)((Rect)(curEnemy.GetObstacleHitbox())).GetHeight());

                        //Picks a Texture2D for the curEnemy, based on the type of enemy
                        Texture2D enemyTexture;
                        if (curEnemy is Nanocloud)
                        {
                            enemyTexture = nanocloudTexture;
                        }
                        else if (curEnemy is Turret)
                        {
                            enemyTexture = turretTexture;
                        }
                        else if (curEnemy is WeepingTurret)
                        {
                            enemyTexture = weepingTurretTexture;
                        }
                        else if (curEnemy is BroodingTurret)
                        {
                            enemyTexture = broodingTurretTexture;
                        }
                        else if (curEnemy is Kelad)
                        {
                            enemyTexture = keladTexture;
                        }
                        else if (curEnemy is Sandman)
                        {
                            enemyTexture = sandmanTexture;
                        }
                        else
                        {
                            enemyTexture = sandpileTexture;
                        }

                        //Draws the enemy
                        sb.Draw(enemyTexture, r, Color.White);
                    }

                    //Initializes data used to draw player
                    Rectangle playerRec = new Rectangle((int)model.GetPlayerCharacter().GetLocation().X, (int)model.GetPlayerCharacter().GetLocation().Y, (int)model.GetPlayerCharacter().GetObstacleHitbox().GetSize(), (int)model.GetPlayerCharacter().GetObstacleHitbox().GetSize());
                    Color playerColour = Color.White;

                    //Visually indicates grace period, if required
                    if (model.GetGame().IsPlayerInGracePeriod() == true)
                    {
                        //Indicates by having the player's character "blink" every 100th of a second
                        if ((model.GetGame().GetInvincibilityTimer() / 10) % 2 == 0)
                        {
                            //Draws the character, with the facing direction dependent on the vector he's currently shooting in
                            if (model.GetWeaponState() == WeaponState.Down || model.GetWeaponState() == WeaponState.None)
                            {
                                sb.Draw(playerFrontTexture, playerRec, playerColour);
                            }
                            else if (model.GetWeaponState() == WeaponState.Left)
                            {
                                sb.Draw(playerLeftTexture, playerRec, playerColour);
                            }
                            else if (model.GetWeaponState() == WeaponState.Up)
                            {
                                sb.Draw(playerBackTexture, playerRec, playerColour);
                            }
                            else
                            {
                                sb.Draw(playerRightTexture, playerRec, playerColour);
                            }
                        }
                    }
                    else
                    {
                        //Draws the character, with the facing direction dependent on the vector he's currently shooting in
                        if (model.GetWeaponState() == WeaponState.Down || model.GetWeaponState() == WeaponState.None)
                        {
                            sb.Draw(playerFrontTexture, playerRec, playerColour);
                        }
                        else if (model.GetWeaponState() == WeaponState.Left)
                        {
                            sb.Draw(playerLeftTexture, playerRec, playerColour);
                        }
                        else if (model.GetWeaponState() == WeaponState.Up)
                        {
                            sb.Draw(playerBackTexture, playerRec, playerColour);
                        }
                        else
                        {
                            sb.Draw(playerRightTexture, playerRec, playerColour);
                        }
                    }
                    
                    break;

                //What to draw if the Model is on the main menu
                case ModelState.MainMenu:

                    //Drawing background
                    sb.Draw(mainMenuBackground, new Rectangle(0, 0, 1200, 675), Color.White);

                    //Drawing buttons
                    this.DrawButtons(sb, model.GetMainMenu().GetAllButtons(), headerFont);

                    break;

                //What to draw if the Model is on the pause menu
                case ModelState.PauseMenu:

                    //Draws background
                    sb.Draw(pauseMenuBackground, new Rectangle(0, 0, 1200, 675), new Color(70, 70, 70));

                    //Draws the pause menu header
                    sb.DrawString(headerFont, "Paused!", new Vector2(50, 20), Color.White);

                    //Draws the pause menu tabs and all buttons from the selected tab
                    this.DrawButtons(sb, model.GetPauseMenu().GetAllButtons(), buttonFont);

                    //Decides what to draw, depending on which tab is open in the pause menu
                    switch (model.GetPauseMenu().GetTab())
                    {
                        case PauseTab.Map:

                            //D̶i̶d̶n̶'̶t̶ ̶h̶a̶v̶e̶ ̶t̶i̶m̶e̶ ̶t̶o̶ ̶m̶a̶k̶e̶ ̶a̶ ̶m̶a̶p̶ ̶s̶y̶s̶t̶e̶m̶
                            //HERE'S YOUR MAP SYSTEM

                            //Loops through each element of the map data
                            for (int y = 0; y < model.GetPauseMenu().GetMapData().GetLength(1); y++)
                            {
                                for (int x = 0; x < model.GetPauseMenu().GetMapData().GetLength(0); x++)
                                {
                                    //Gets map icon file path
                                    string mapFilePath = @"Map\" + model.GetPauseMenu().GetMapData()[x, y];

                                    //Only runs if file was found
                                    if(!mapFilePath.Equals(@"Map\"))
                                    {
                                        //Highlights current room
                                        if (x == model.GetPauseMenu().GetMapData().GetLength(0) / 2 && y == model.GetPauseMenu().GetMapData().GetLength(1) / 2)
                                        {
                                            //Draws map icon
                                            sb.Draw(contentManager.Load<Texture2D>(mapFilePath), new Rectangle(x * MAP_ICON_SIZE + MAP_ICON_X_OFFSET, y * MAP_ICON_SIZE + MAP_ICON_Y_OFFSET, MAP_ICON_SIZE, MAP_ICON_SIZE), Color.White);
                                        }
                                        else
                                        {
                                            //Draws map icon
                                            sb.Draw(contentManager.Load<Texture2D>(mapFilePath), new Rectangle(x * MAP_ICON_SIZE + MAP_ICON_X_OFFSET, y * MAP_ICON_SIZE + MAP_ICON_Y_OFFSET, MAP_ICON_SIZE, MAP_ICON_SIZE), Color.Gray);
                                        }
                                    }
                                }
                            }

                            break;

                        case PauseTab.Stats:

                            //Draws stats as numbers
                            sb.DrawString(buttonFont, "DAMAGE: " + model.GetPlayerCharacter().GetStats().GetDamage(), new Vector2(100, 240), Color.Pink);
                            sb.DrawString(buttonFont, "SPEED: " + model.GetPlayerCharacter().GetStats().GetSpeed(), new Vector2(100, 290), Color.LightBlue);
                            sb.DrawString(buttonFont, "RANGE: " + model.GetPlayerCharacter().GetStats().GetRange(), new Vector2(100, 340), Color.WhiteSmoke);
                            sb.DrawString(buttonFont, "FIRERATE: " + model.GetPlayerCharacter().GetStats().GetFireRate(), new Vector2(100, 390), Color.LightGoldenrodYellow);
                            sb.DrawString(buttonFont, "HEALTH: " + model.GetPlayerCharacter().GetStats().GetHealth(), new Vector2(100, 440), Color.Crimson);

                            break;

                        case PauseTab.Inventory:

                            //Draws the selected item
                            Rectangle invRec = new Rectangle(600, 110, 300, 300);
                            this.DrawItem(sb, invRec, model.GetPauseMenu().GetInvMenu().GetCurrentItem().GetEffect());

                            sb.DrawString(headerFont, model.GetPauseMenu().GetInvMenu().GetCurrentItem().GetName(), new Vector2(600, 420), Color.LightGray);
                            sb.DrawString(descFont, "Effect: " + model.GetPauseMenu().GetInvMenu().GetCurrentItem().GetEffectDesc(), new Vector2(600, 470), Color.White);
                            sb.DrawString(descFont, '"' + model.GetPauseMenu().GetInvMenu().GetCurrentItem().GetItemDesc() + '"', new Vector2(600, 500), Color.White);

                            //Draws the amount of gold the player currently has
                            sb.DrawString(buttonFont, model.GetPlayerCharacter().GetStats().GetGold() + "G", new Vector2(970, 110), Color.White);

                            //Tells the player that they cannot sell their last item
                            if (model.GetPlayerCharacter().GetStats().GetPassiveEffects().Count == 1)
                            {
                                sb.DrawString(descFont, "You cannot sell your last item.", new Vector2(50, 300), Color.White);
                            }
                            
                            break;

                        case PauseTab.Shop:

                            //Draws the selected item
                            Rectangle shopRec = new Rectangle(600, 110, 300, 300);
                            this.DrawItem(sb, shopRec, model.GetPauseMenu().GetShopMenu().GetCurrentItem().GetEffect());

                            //Draws the info about the item
                            sb.DrawString(headerFont, model.GetPauseMenu().GetShopMenu().GetCurrentItem().GetName(), new Vector2(600, 420), Color.LightGray);
                            sb.DrawString(descFont, "Effect: " + model.GetPauseMenu().GetShopMenu().GetCurrentItem().GetEffectDesc(), new Vector2(600, 470), Color.White);
                            sb.DrawString(descFont, '"' + model.GetPauseMenu().GetShopMenu().GetCurrentItem().GetItemDesc() + '"', new Vector2(600, 500), Color.White);

                            //Draws the amount of gold the player currently has
                            sb.DrawString(buttonFont, model.GetPlayerCharacter().GetStats().GetGold() + "G", new Vector2(970, 110), Color.White);

                            break;

                    }

                    break;

                case ModelState.GameOver:

                    //Draws background
                    sb.Draw(gameOverBackground, new Rectangle(0, 0, 1200, 675), Color.White);

                    //Draws buttons
                    this.DrawButtons(sb, model.GetGameOverMenu().GetAllButtons(), headerFont);

                    break;
            }

            //Closes the spritebatch
            sb.End();
        }

        /// <summary>
        /// Draws a specified item in a specified Rectangle
        /// </summary>
        /// <param name="sb">A valid Spritebatch (Must have already called Begin method)</param>
        /// <param name="drawRec">A valid Rectangle</param>
        /// <param name="effect">A valid PassiveEffect</param>
        private void DrawItem(SpriteBatch sb, Rectangle drawRec, PassiveEffect effect)
        {
            //Draws a texture, chosen based on the given PassiveEffect
            switch (effect)
            {
                case PassiveEffect.Damage_FocusLens:

                    sb.Draw(lensTexture, drawRec, Color.White);

                    break;

                case PassiveEffect.Damage_Graphene:

                    sb.Draw(grapheneTexture, drawRec, Color.White);

                    break;

                case PassiveEffect.Damage_Nanomaterial:

                    sb.Draw(nanoTexture, drawRec, Color.White);

                    break;

                case PassiveEffect.Damage_SapphireCrystal:

                    sb.Draw(crystalTexture, drawRec, Color.White);

                    break;

                case PassiveEffect.FireRate_Overclocked:

                    sb.Draw(overclockedTexture, drawRec, Color.White);

                    break;

                case PassiveEffect.FireRate_RoboOnion:

                    sb.Draw(onionTexture, drawRec, Color.White);

                    break;

                case PassiveEffect.FireRate_TitanX:

                    sb.Draw(titanXTexture, drawRec, Color.White);

                    break;

                case PassiveEffect.Health_Chess:

                    sb.Draw(chessTexture, drawRec, Color.White);

                    break;

                case PassiveEffect.Health_FaradayArmour:

                    sb.Draw(faradayTexture, drawRec, Color.White);

                    break;

                case PassiveEffect.Health_LiIonBattery:

                    sb.Draw(batteryTexture, drawRec, Color.White);

                    break;

                case PassiveEffect.Health_NuclearPower:

                    sb.Draw(nuclearTexture, drawRec, Color.White);

                    break;

                case PassiveEffect.Homing_HAL:

                    sb.Draw(skynetTexture, drawRec, Color.White);

                    break;

                case PassiveEffect.QuadShot_QuadCore:

                    sb.Draw(quadCoreTexture, drawRec, Color.White);

                    break;

                case PassiveEffect.Range_SilphScope:

                    sb.Draw(scopeTexture, drawRec, Color.White);

                    break;

                case PassiveEffect.Speed_AluminumArmour:

                    sb.Draw(aluminumTexture, drawRec, Color.White);

                    break;

                case PassiveEffect.Speed_LiquidCooling:

                    sb.Draw(coolingTexture, drawRec, Color.White);

                    break;
            }
        }

        /// <summary>
        /// Draws all buttons in the given list
        /// </summary>
        /// <param name="sb">A valid SpriteBatch</param>
        /// <param name="buttons">A valid List of Buttons</param>
        /// <param name="textFont">A valid SpriteFont</param>
        private void DrawButtons(SpriteBatch sb, List<Button> buttons, SpriteFont textFont)
        {
            foreach (Button button in buttons)
            {
                sb.Draw(buttonTexture, button.GetBox(), button.GetColour());
                sb.DrawString(textFont, button.GetText(), button.GetTextPos(), Color.White);
            }
        }
    }
}
