using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using XELibrary;

namespace ExplosionDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private InputHandler input;
        private CelAnimationManager cam;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            input = new InputHandler(this);
            Components.Add(input);

            cam = new CelAnimationManager(this, @"Textures\");
            Components.Add(cam);
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
            spriteBatch = new SpriteBatch(GraphicsDevice);

            cam.AddAnimation("explosion", "explode_1", new CelCount(4, 4), 16);
            cam.AddAnimation("explosion2", "explode_1", new CelCount(4, 4), 16);
            cam.AddAnimation("explosion3", "explode_3", new CelCount(4, 4), 12);
            cam.AddAnimation("explosion4", "explode_4", new CelCount(4, 4), 20);
            cam.AddAnimation("explosion5", "explode_3", new CelCount(4, 4), 12);
            cam.AddAnimation("explosion6", "explode_4", new CelCount(4, 4), 20);

            cam.AddAnimation("bigexplosion", "bigexplosion", new CelCount(4, 4), 18);

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
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            cam.Draw(gameTime, "explosion", spriteBatch, new Vector2(32, 32));
            cam.Draw(gameTime, "explosion2", spriteBatch, new Vector2(40, 40));
            cam.Draw(gameTime, "explosion3", spriteBatch, new Vector2(64, 32));
            cam.Draw(gameTime, "explosion4", spriteBatch, new Vector2(64, 64));
            cam.Draw(gameTime, "explosion5", spriteBatch, new Vector2(28, 40));
            cam.Draw(gameTime, "explosion6", spriteBatch, new Vector2(40, 64));

            cam.Draw(gameTime, "bigexplosion", spriteBatch, new Vector2(150, 150));
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
