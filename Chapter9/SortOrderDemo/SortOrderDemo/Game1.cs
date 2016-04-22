using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using XELibrary;

namespace SortOrderDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Texture2D tiledSprite;

        private BlendState blendState = BlendState.AlphaBlend;
        private SpriteSortMode sortMode = SpriteSortMode.Deferred;
        private InputHandler input;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            input = new InputHandler(this);
            Components.Add(input);

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

            tiledSprite = Content.Load<Texture2D>(@"Textures\shapes");
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

            if (WasPressed(Buttons.A, Keys.A))
                blendState = BlendState.AlphaBlend;
            if (WasPressed(Buttons.B, Keys.B))
                blendState = BlendState.Additive;
            if (WasPressed(Buttons.X, Keys.X))
                blendState = BlendState.NonPremultiplied;

            if (WasPressed(Buttons.LeftShoulder, Keys.D1))
                sortMode = SpriteSortMode.BackToFront;
            if (WasPressed(Buttons.RightShoulder, Keys.D2))
                sortMode = SpriteSortMode.FrontToBack;
            if (WasPressed(Buttons.LeftStick, Keys.D3))
                sortMode = SpriteSortMode.Deferred;
            if (WasPressed(Buttons.RightStick, Keys.D4))
                sortMode = SpriteSortMode.Immediate;
            if (WasPressed(Buttons.Y, Keys.D5))
                sortMode = SpriteSortMode.Texture;


            base.Update(gameTime);
        }

        private bool WasPressed(Buttons buttonType, Keys keys)
        {
            return (WasPressed(0, buttonType, keys));
        }

        private bool WasPressed(int playerIndex, Buttons buttonType,
            Keys keys)
        {
            if (input.ButtonHandler.WasButtonPressed(playerIndex, buttonType) ||
                input.KeyboardState.WasKeyPressed(keys))
                return (true);
            else
                return (false);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(sortMode, blendState);

            //draw heart
            spriteBatch.Draw(tiledSprite, new Rectangle(64, 64, 256, 256),
                new Rectangle(256, 256, 256, 256), Color.White, 0, Vector2.Zero,
                SpriteEffects.None, .10f);

            //draw circle
            spriteBatch.Draw(tiledSprite, new Rectangle(0, 0, 256, 256),
                new Rectangle(256, 0, 256, 256), Color.White, 0, Vector2.Zero,
                SpriteEffects.None, .15f);

            //draw shape
            spriteBatch.Draw(tiledSprite, new Rectangle(128, 128, 256, 256),
                new Rectangle(0, 0, 256, 256), Color.White, 0, Vector2.Zero,
                SpriteEffects.None, .05f);

            //draw star
            spriteBatch.Draw(tiledSprite, new Rectangle(192, 192, 256, 256),
                new Rectangle(0, 256, 256, 256), Color.White, 0, Vector2.Zero,
                SpriteEffects.None, .01f);

            Window.Title = "Sort Order Demo - " + blendState.ToString() + " : " +
                sortMode.ToString();

            spriteBatch.End();
            base.Draw(gameTime);

        }
    }
}
