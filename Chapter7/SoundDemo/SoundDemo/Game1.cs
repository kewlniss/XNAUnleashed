using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using Microsoft.Xna.Framework.Media;

using XELibrary;

namespace SoundDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private InputHandler input;
        private SoundManager sound;

        private float currentVolume = 0.5f;
        private float value = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            input = new InputHandler(this);
            Components.Add(input);

            sound = new SoundManager(this, "Chapter7");
            Components.Add(sound);

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

            string[] playList = { "Song1", "Song2", "Song3" };
            sound.StartPlayList(playList);

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
            if (input.KeyboardState.WasKeyPressed(Keys.D1) ||
                    input.ButtonHandler.WasButtonPressed(0, Buttons.A))
                sound.Play("gunshot");
            if (input.KeyboardState.WasKeyPressed(Keys.D2) ||
                    input.ButtonHandler.WasButtonPressed(0, Buttons.B))
                sound.Play("hit");
            if (input.KeyboardState.WasKeyPressed(Keys.D3) ||
                    input.ButtonHandler.WasButtonPressed(0,
                        Buttons.LeftShoulder))
                sound.Play("attention");
            if (input.KeyboardState.WasKeyPressed(Keys.D4) ||
                    input.ButtonHandler.WasButtonPressed(0,
                        Buttons.LeftStick))
                sound.Play("explosion");
            if (input.KeyboardState.WasKeyPressed(Keys.D5) ||
                    input.ButtonHandler.WasButtonPressed(0,
                        Buttons.RightShoulder))
                sound.Play("bullet");
            if (input.KeyboardState.WasKeyPressed(Keys.D6) ||
                    input.ButtonHandler.WasButtonPressed(0,
                        Buttons.RightStick))
                sound.Play("crash");
            if (input.KeyboardState.WasKeyPressed(Keys.D7) ||
                    input.ButtonHandler.WasButtonPressed(0, Buttons.X))
                sound.Play("complex");
            if (input.KeyboardState.WasKeyPressed(Keys.D8) ||
                    input.ButtonHandler.WasButtonPressed(0, Buttons.Y))
                sound.Toggle("CoolLoop");
            if (input.KeyboardState.WasKeyPressed(Keys.D9) ||
                    input.ButtonHandler.WasButtonPressed(0,
                        Buttons.LeftShoulder))
                sound.Toggle("CoolLoop 2");

            if (input.KeyboardState.WasKeyPressed(Keys.P) ||
                input.ButtonHandler.WasButtonPressed(0, Buttons.Start))
            {
                sound.Toggle("CoolLoop");
            }

            if (input.KeyboardState.WasKeyPressed(Keys.S) ||
                    (input.GamePads[0].Triggers.Right > 0))
                sound.StopPlayList();


            if (input.KeyboardState.IsHoldingKey(Keys.Up) ||
                    input.GamePads[0].DPad.Up == ButtonState.Pressed)
                currentVolume += 0.05f;
            if (input.KeyboardState.IsHoldingKey(Keys.Down) ||
                    input.GamePads[0].DPad.Down == ButtonState.Pressed)
                currentVolume -= 0.05f;

            currentVolume = MathHelper.Clamp(currentVolume, 0.0f, 1.0f);
            sound.SetVolume("Default", currentVolume);

            if (input.KeyboardState.WasKeyPressed(Keys.NumPad1))
                value = 5000;
            if (input.KeyboardState.WasKeyPressed(Keys.NumPad2))
                value = 25000;
            if (input.KeyboardState.WasKeyPressed(Keys.NumPad3))
                value = 30000;
            if (input.KeyboardState.WasKeyPressed(Keys.NumPad4))
                value = 40000;
            if (input.KeyboardState.WasKeyPressed(Keys.NumPad5))
                value = 50000;
            if (input.KeyboardState.WasKeyPressed(Keys.NumPad6))
                value = 60000;
            if (input.KeyboardState.WasKeyPressed(Keys.NumPad7))
                value = 70000;
            if (input.KeyboardState.WasKeyPressed(Keys.NumPad8))
                value = 80000;
            if (input.KeyboardState.WasKeyPressed(Keys.NumPad9))
                value = 90000;
            if (input.KeyboardState.WasKeyPressed(Keys.NumPad0))
                value = 100000;

            if (input.GamePads[0].Triggers.Left > 0)
                value = input.GamePads[0].Triggers.Left * 100000;

            sound.SetGlobalVariable("SpeedOfSound", value);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
