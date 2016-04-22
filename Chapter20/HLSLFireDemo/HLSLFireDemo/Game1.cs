using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using XELibrary;

namespace HLSLFireDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private InputHandler input;
        private FPS fps;

        private Effect fireEffect;
        private Random rand = new Random();

        private Texture2D hotSpotTexture;
        private Texture2D fire;
        private Rectangle tileSafeArea;

        private RenderTarget2D renderTarget1;
        private RenderTarget2D renderTarget2;

        private int offset = -128;
        private Color[] colors = {
            Color.Black,
            Color.Yellow,
            Color.White,
            Color.Red,
            Color.Orange,
            new Color(255,255,128) //yellowish white
        };


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            input = new InputHandler(this, true);
            Components.Add(input);

            fps = new FPS(this, true, true);
            Components.Add(fps);

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

            fps.Load(spriteBatch);

            tileSafeArea = Utility.GetTitleSafeArea(GraphicsDevice, .8f);
            hotSpotTexture = CreateTexture(4, 1);

            fireEffect = Content.Load<Effect>(@"Effects\Fire");

            renderTarget1 = new RenderTarget2D(GraphicsDevice,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, true, SurfaceFormat.Color, DepthFormat.None);// DepthFormat.Depth24Stencil8);

            renderTarget2 = new RenderTarget2D(GraphicsDevice,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, true, SurfaceFormat.Color, DepthFormat.None); //);

            fire = null;

        }

        private Texture2D CreateTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(graphics.GraphicsDevice, width, height, false, SurfaceFormat.Color);

            int pixelCount = width * height;
            Color[] pixelData = new Color[pixelCount];

            for (int i = 0; i < pixelCount; i++)
                pixelData[i] = Color.White;

            texture.SetData(pixelData);

            return (texture);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            renderTarget1.Dispose();
            renderTarget2.Dispose();
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

            GraphicsDevice device = graphics.GraphicsDevice;
            /*
            //test hotSpotTexture
            device.Clear(Color.Black);

            spriteBatch.Begin();
            for (int i = 0; i < device.Viewport.Width / hotSpotTexture.Width; i++)
            {
                spriteBatch.Draw(hotSpotTexture,
                    new Vector2(i * hotSpotTexture.Width,
                        device.Viewport.Height - hotSpotTexture.Height),
                    colors[rand.Next(colors.Length)]);
            }

            //spriteBatch.Draw(hotSpotTexture, new Vector2(50, 50), Color.White);
                //new Vector2(i * hotSpotTexture.Width,
                //    device.Viewport.Height - hotSpotTexture.Height),
                //colors[rand.Next(colors.Length)]);

            spriteBatch.End();

            base.Draw(gameTime);
            return;
            */
            //Draw hotspots on the first Render Target
            device.SetRenderTarget(renderTarget1);
            device.Clear(Color.Black);

            spriteBatch.Begin();

            //get last drawn screen — if not first time in
            //fire is null first time in, and when device is lost (LoadGraphicsContent)
            if (fire != null) //render target have valid texture
                spriteBatch.Draw(fire, Vector2.Zero, Color.White);

            //draw hotspots
            for (int i = 0; i < device.Viewport.Width / hotSpotTexture.Width; i++)
            {
                spriteBatch.Draw(hotSpotTexture,
                    new Vector2(i * hotSpotTexture.Width,
                        device.Viewport.Height - hotSpotTexture.Height),
                    colors[rand.Next(colors.Length)]);
            }

            spriteBatch.End();

            //resolve what we just drew to our render target
            //clear it out
            device.SetRenderTarget(null);

            // Transfer from first to second render target
            device.SetRenderTarget(renderTarget2);

            //Either this
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, null, null, fireEffect);

            //or this is valid
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            //EffectPass pass = fireEffect.CurrentTechnique.Passes[0];
            //pass.Apply();

            spriteBatch.Draw(renderTarget1,
                new Rectangle(0, offset, device.Viewport.Width,
                device.Viewport.Height - offset), Color.White);

            spriteBatch.End();

            //resolve what we just drew to our render target
            //clear it out
            device.SetRenderTarget(null);

            device.Clear(Color.Black);

            //set texture to render
            fire = renderTarget2;

            // Draw second render target onto the screen (back buffer)
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            //render texture three times (in additive mode) to saturate color
            spriteBatch.Draw(fire, tileSafeArea, Color.White);
            spriteBatch.Draw(fire, tileSafeArea, Color.White);
            spriteBatch.Draw(fire, tileSafeArea, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);

        }
    }
}
