using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace RotateAndScaleDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Texture2D circular;
        private Rectangle destination;
        private Rectangle source;
        private float rotation;
        private Vector2 origin;
        private float scale;

        private float fadeAmount;
        private Texture2D fadeTexture;

        BlendState[] blendState = new BlendState[4];


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            blendState[0] = new BlendState();
            blendState[0].ColorSourceBlend = Blend.DestinationColor;
            blendState[0].ColorDestinationBlend = Blend.SourceColor;

            blendState[1] = new BlendState();
            blendState[1].ColorSourceBlend = Blend.One;
            blendState[1].ColorDestinationBlend = Blend.One;

            blendState[2] = new BlendState();
            blendState[2].ColorSourceBlend = Blend.DestinationColor;
            blendState[2].ColorDestinationBlend = Blend.SourceAlpha;

            blendState[3] = new BlendState();
            blendState[3].ColorSourceBlend = Blend.InverseSourceColor;
            blendState[3].ColorDestinationBlend = Blend.InverseDestinationColor;

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

            circular = Content.Load<Texture2D>(@"Textures\circular");

            destination = new Rectangle(graphics.PreferredBackBufferWidth / 2,
                graphics.PreferredBackBufferHeight / 2, circular.Width, circular.Height);
            origin = new Vector2(circular.Width / 2, circular.Height / 2);
            source = new Rectangle(0, 0, circular.Width, circular.Height);

            rotation = 1.0f;
            scale = 0.5f;

            fadeTexture = CreateFadeTexture(GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private Texture2D CreateFadeTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(
                GraphicsDevice, width, height, true,
                SurfaceFormat.Color);

            int pixelCount = width * height;
            Color[] pixelData = new Color[pixelCount];
            Random rnd = new Random();

            for (int i = 0; i < pixelCount; i++)
            {
                //could fade to a different color
                pixelData[i] = Color.Black;
            }

            texture.SetData(pixelData);

            return (texture);
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

            rotation += .05f;

            //start fading out after 2 seconds
            if (gameTime.TotalGameTime.Seconds > 2)
                fadeAmount += (.0005f * gameTime.ElapsedGameTime.Milliseconds);

            //reset fade amount after a short time to see the effect again
            if (fadeAmount > 2.0f) //two seconds passed?
                fadeAmount = 0.0f;

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            graphics.GraphicsDevice.BlendState = blendState[0];

            spriteBatch.Draw(circular, destination, source, Color.White, rotation, origin,
                SpriteEffects.None, 0.0f);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            graphics.GraphicsDevice.BlendState = blendState[1];

            spriteBatch.Draw(circular, new Vector2(600, 100), null, Color.White, -rotation,
                new Vector2(256, 256), scale, SpriteEffects.None, 0.0f);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            graphics.GraphicsDevice.BlendState = blendState[2];

            spriteBatch.Draw(circular, new Vector2(100, 600), null, Color.White, -rotation,
                new Vector2(256, 256), scale, SpriteEffects.None, 0.0f);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            graphics.GraphicsDevice.BlendState = blendState[3];

            spriteBatch.Draw(circular, new Vector2(300, 300), null, Color.White, -rotation,
                new Vector2(0, 0), scale, SpriteEffects.None, 0.0f);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw(fadeTexture, Vector2.Zero,
                new Color(new Vector4(Color.White.ToVector3(), fadeAmount)));
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
