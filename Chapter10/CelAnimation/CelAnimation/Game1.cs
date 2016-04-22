using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using XELibrary;

namespace CelAnimation
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private CelAnimationManager cam;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

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

            cam.AddAnimation("enemy1", "MrEye", new CelCount(4, 2), 8);
            cam.AddAnimation("enemy2", "MrEye", new CelCount(4, 2), 12);
            cam.AddAnimation("enemy3", "MrEye", new CelCount(4, 2), 6);

            cam.AddAnimation("complex1", "complex", new CelRange(1, 1, 2, 1), 64, 64, 2, 2);
            cam.AddAnimation("complex2", "complex", new CelRange(3, 1, 1, 3), 64, 64, 7, 8);
            cam.AddAnimation("complex3", "complex", new CelRange(2, 3, 1, 4), 64, 64, 4, 2);
            cam.AddAnimation("complex4", "complex", new CelRange(2, 4, 4, 4), 64, 64, 3, 5);

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

            spriteBatch.Begin();
            cam.Draw(gameTime, "enemy1", spriteBatch, new Vector2(50, 50));
            cam.Draw(gameTime, "enemy2", spriteBatch, new Vector2(150, 75));
            cam.Draw(gameTime, "enemy3", spriteBatch, new Vector2(70, 130));

            cam.Draw(gameTime, "complex1", spriteBatch, new Vector2(400, 50));
            cam.Draw(gameTime, "complex2", spriteBatch, new Vector2(400, 150));
            cam.Draw(gameTime, "complex3", spriteBatch, new Vector2(400, 250));
            cam.Draw(gameTime, "complex4", spriteBatch, new Vector2(400, 350));
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
