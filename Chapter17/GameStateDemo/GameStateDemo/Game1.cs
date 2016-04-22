using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using XELibrary;

namespace GameStateDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        public SpriteBatch SpriteBatch;

        private InputHandler input;
        private Camera camera;
        private GameStateManager gameManager;

        public ITitleIntroState TitleIntroState;
        public IStartMenuState StartMenuState;
        public IOptionsMenuState OptionsMenuState;
        public IPlayingState PlayingState;
        public IStartLevelState StartLevelState;
        public ILostGameState LostGameState;
        public IWonGameState WonGameState;
        public IFadingState FadingState;
        public IPausedState PausedState;
        public IYesNoDialogState YesNoDialogState;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            input = new InputHandler(this);
            Components.Add(input);

            camera = new Camera(this);
            Components.Add(camera);

            gameManager = new GameStateManager(this);
            Components.Add(gameManager);

            TitleIntroState = new TitleIntroState(this);
            StartMenuState = new StartMenuState(this);
            OptionsMenuState = new OptionsMenuState(this);
            PlayingState = new PlayingState(this);
            StartLevelState = new StartLevelState(this);
            FadingState = new FadingState(this);
            LostGameState = new LostGameState(this);
            WonGameState = new WonGameState(this);
            PausedState = new PausedState(this);
            YesNoDialogState = new YesNoDialogState(this);

            gameManager.ChangeState(TitleIntroState.Value);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            SpriteBatch.Begin();

            base.Draw(gameTime);
            
            SpriteBatch.End();
        }
    }
}
