using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using XELibrary;

namespace SimpleGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SimpleGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public enum GameState { StartMenu, Scene, Lost, Won }
        public GameState State = GameState.StartMenu;
        private InputHandler input;
        private Texture2D startMenu;
        private Vector2 centerVector;
        private int viewportWidth;
        private int viewportHeight;

        private Background background;
        private ScrollingBackgroundManager sbm;

        private Player player;
        private Vector2 playerPosition = new Vector2(64, 350);
        private int width = 640;
        private int height = 480;

        private CelAnimationManager cam;

        private EnemyManager enemies;

        private bool paused;

        private FadeOut fade;

        private ExplosionManager explosionManager;

        //private SoundManager sound;

        public SimpleGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;

            input = new InputHandler(this);
            Components.Add(input);

            background = new Background(this, @"Textures\");
            background.Enabled = background.Visible = false;
            Components.Add(background);

            cam = new CelAnimationManager(this, @"Textures\");
            Components.Add(cam);

            player = new Player(this);
            player.Position = playerPosition;
            Components.Add(player);

            enemies = new EnemyManager(this);
            Components.Add(enemies);

            fade = new FadeOut(this);
            Components.Add(fade);

            explosionManager = new ExplosionManager(this);
            Components.Add(explosionManager);

            //sound = new SoundManager(this, "Chapter7");
            //Components.Add(sound);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            viewportHeight = graphics.GraphicsDevice.Viewport.Height;
            viewportWidth = graphics.GraphicsDevice.Viewport.Width;

            //string[] playList = { "Song1", "Song2", "Song3" };
            //sound.StartPlayList(playList);

            base.Initialize();

            //The background game component creates this
            //we want access so we get the service after the components initialize
            sbm = (ScrollingBackgroundManager)Services.GetService(
                                  typeof(IScrollingBackgroundManager));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            startMenu = Content.Load<Texture2D>(@"Textures\startmenu");
            centerVector = new Vector2((viewportWidth - startMenu.Width) / 2,
                (viewportHeight - startMenu.Height) / 2);

            background.Load(spriteBatch);
            player.Load(spriteBatch);
            fade.Load(spriteBatch);
            explosionManager.Load(spriteBatch);

            ActivateStartMenu();

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            switch (State)
            {
                case GameState.Scene:
                    {
                        UpdateScene(gameTime);
                        break;
                    }
                case GameState.StartMenu:
                    {
                        UpdateStartMenu();
                        break;
                    }
                case GameState.Won:
                    {
                        //Game Over - You Won!
                        if (!fade.Enabled)
                        {
                            fade.Color = Color.Black;
                            fade.Enabled = true;
                        }
                        break;
                    }
                case GameState.Lost:
                    {
                        if (!fade.Enabled)
                        {
                            fade.Color = Color.Red;
                            fade.Enabled = true;
                        }
                        break;
                    }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            if (State == GameState.StartMenu)
                DrawStartMenu();

            base.Draw(gameTime);

            //Display our foreground (after game components)
            if (State == GameState.Scene)
                sbm.Draw("foreground", spriteBatch);
            
            spriteBatch.End();
        }

        private void DrawStartMenu()
        {
            spriteBatch.Draw(startMenu, centerVector, Color.White);
        }

        private bool WasPressed(int playerIndex, Buttons button, Keys keys)
        {
            if (input.ButtonHandler.WasButtonPressed(playerIndex, button) ||
                    input.KeyboardState.WasKeyPressed(keys))
                return (true);
            else
                return (false);
        }

        private void UpdateScene(GameTime gameTime)
        {
            CheckForCollisions(gameTime);

            if (!enemies.EnemiesExist)
            {
                //Level over  (would advance level here and on last level state won game)
                enemies.SetPause(true);
                enemies.Enabled = false;
                player.SetPause(true);
                player.Enabled = false;
                background.Enabled = false;
                State = GameState.Won;
            }

            if (WasPressed(0, Buttons.Start, Keys.Enter))
            {
                ActivateStartMenu();
            }
        }

        private void ActivateGame()
        {
            background.Visible = background.Enabled = true; //start updating scene
            State = GameState.Scene;
            player.Visible = player.Enabled = true;
            player.SetPause(false);

            if (!paused)
            {
                enemies.Load(spriteBatch, 5, 20, 120.0f); //should be read by level (level 1)
                explosionManager.SetMaxNumberOfExplosions(5); //should be read by level
            }
            enemies.Visible = enemies.Enabled = true; // resume updating enemies
            enemies.SetPause(false);

        }


        private void UpdateStartMenu()
        {
            if (WasPressed(0, Buttons.Start, Keys.Enter))
            {
                paused = false;
                ActivateGame();
            }
        }

        public void ActivateStartMenu()
        {
            //stop updating scene backgrounds
            background.Visible = background.Enabled = false;
            State = GameState.StartMenu;
            player.Visible = player.Enabled = false;
            player.SetPause(true);
            enemies.Visible = enemies.Enabled = false; //stop updating enemies
            enemies.SetPause(true);
            paused = true;
        }

        private void CheckForCollisions(GameTime gameTime)
        {
            int enemyIndex = enemies.CollidedWithPlayer(player.Position);
            if (enemyIndex != -1)
            {
                if (player.Attacking)
                {
                    //die robot 
                    Vector2 enemyPosition = enemies.Die(enemyIndex);

                    //and explode
                    explosionManager.StartExplosion(enemyIndex, enemyPosition,
                        gameTime.TotalGameTime.TotalMilliseconds);

                    //complete with sound
                    //sound.Play("explosion");
                }
                else
                {
                    background.Enabled = false;
                    enemies.SetPause(true);
                    enemies.Enabled = false;
                    player.SetPause(true);
                    player.Enabled = false;
                    State = GameState.Lost;
                }
            }
        }

    }
}
