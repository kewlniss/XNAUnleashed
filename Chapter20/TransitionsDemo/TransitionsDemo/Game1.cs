using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using XELibrary;

namespace TransitionsDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private enum TransitionState { None, CrossFade, Wipe };

        private FirstPersonCamera camera;
        private InputHandler input;

        private Model model;
        private Texture2D texture;
        private Effect effect;

        private Texture2D splashScreen;

        private RenderTarget2D renderTarget;

        private TransitionState state = TransitionState.CrossFade;

        private float fadeAmount;

        private Rectangle wipeInDestinationRectangle;
        private Rectangle wipeInSourceRectangle;
        private Rectangle wipeOutDestinationRectangle;
        private Rectangle wipeOutSourceRectangle;
        private enum WipeDirection { Left, Right, Up, Down };
        private WipeDirection wipeDirection = WipeDirection.Left;
        private int wipeX, wipeY, wipeWidth, wipeHeight;
        private float wipeAmount;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            input = new InputHandler(this, true);
            Components.Add(input);

            camera = new FirstPersonCamera(this);
            Components.Add(camera);
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

            InitializeValues();

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

            model = Content.Load<Model>(@"Models\asteroid1");
            texture = Content.Load<Texture2D>(@"Textures\asteroid1");
            effect = Content.Load<Effect>(@"Effects\AmbientTexture");
            splashScreen = Content.Load<Texture2D>(@"Textures\splashscreen");


            renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, 
                DepthFormat.Depth24Stencil8);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            renderTarget.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            UpdateInput();

            switch (state)
            {
                case TransitionState.CrossFade:
                    {
                        UpdateFade(gameTime);
                        break;
                    }
                case TransitionState.Wipe:
                    {
                        UpdateWipe(gameTime);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            base.Update(gameTime);

        }
        private void UpdateInput()
        {
            if (input.KeyboardState.WasKeyPressed(Keys.Space) ||
                input.ButtonHandler.WasButtonPressed(0, Buttons.A))
            {
                state--;

                if (state < TransitionState.None)
                    state = TransitionState.Wipe;

                InitializeValues();
            }
        }

        private void InitializeValues()
        {
            fadeAmount = 0;
            wipeAmount = 0;
        }

        private void UpdateFade(GameTime gameTime)
        {
            fadeAmount += (.0005f * gameTime.ElapsedGameTime.Milliseconds);
            
            //reset fade amount after a short time to see the effect again
            if (fadeAmount > 2.0f)
                fadeAmount = 0.0f;
        }

        private void UpdateWipe(GameTime gameTime)
        {
            Viewport viewport = graphics.GraphicsDevice.Viewport;

            wipeAmount += (0.5f * (float)gameTime.ElapsedGameTime.Milliseconds);

            switch (wipeDirection)
            {
                case WipeDirection.Left:
                    {
                        WipeLeft(viewport);
                        break;
                    }
                case WipeDirection.Right:
                    {
                        WipeRight(viewport);
                        break;
                    }
                case WipeDirection.Up:
                    {
                        WipeUp(viewport);
                        break;
                    }
                case WipeDirection.Down:
                    {
                        WipeDown(viewport);
                        break;
                    }
            }
        }

        private void WipeDown(Viewport viewport)
        {
            wipeY = 0;
            wipeX = 0;
            wipeWidth = viewport.Width;
            wipeHeight = Convert.ToInt32(wipeAmount);

            wipeInDestinationRectangle = new Rectangle(0, 0, wipeWidth, wipeHeight);
            wipeInSourceRectangle = new Rectangle(0, 0, wipeWidth, wipeHeight);
            wipeOutDestinationRectangle = new Rectangle(wipeX, wipeHeight, wipeWidth,
                viewport.Height - wipeHeight);
            wipeOutSourceRectangle = new Rectangle(wipeX, wipeHeight, wipeWidth,
                viewport.Height - wipeHeight);

            if (wipeAmount > viewport.Height)
                ChangeWipe();
        }

        private void WipeUp(Viewport viewport)
        {
            wipeY = Convert.ToInt32(viewport.Height - wipeAmount);
            wipeX = 0;
            wipeWidth = viewport.Width;
            wipeHeight = Convert.ToInt32(wipeAmount);

            wipeInDestinationRectangle = new Rectangle(wipeX, wipeY, wipeWidth,
                wipeHeight);
            wipeInSourceRectangle = new Rectangle(wipeX, wipeY, wipeWidth, wipeHeight);
            wipeOutDestinationRectangle = new Rectangle(0, 0, wipeWidth, wipeY);
            wipeOutSourceRectangle = new Rectangle(0, 0, wipeWidth, wipeY);

            if (wipeAmount > viewport.Height)
                ChangeWipe();
        }

        private void WipeRight(Viewport viewport)
        {
            wipeY = 0;
            wipeX = 0;
            wipeWidth = Convert.ToInt32(wipeAmount);
            wipeHeight = viewport.Height;

            wipeInDestinationRectangle = new Rectangle(0, 0, wipeWidth, wipeHeight);
            wipeInSourceRectangle = new Rectangle(0, 0, wipeWidth, wipeHeight);
            wipeOutDestinationRectangle = new Rectangle(wipeWidth, wipeY,
                viewport.Width - wipeWidth, wipeHeight);
            wipeOutSourceRectangle = new Rectangle(wipeWidth, wipeY,
                viewport.Width - wipeWidth, wipeHeight);

            if (wipeAmount > viewport.Width)
                ChangeWipe();
        }

        private void WipeLeft(Viewport viewport)
        {
            wipeY = 0;
            wipeX = Convert.ToInt32(viewport.Width - wipeAmount);
            wipeWidth = Convert.ToInt32(wipeAmount);
            wipeHeight = viewport.Height;

            wipeInDestinationRectangle = new Rectangle(wipeX, wipeY, wipeWidth,
                wipeHeight);
            wipeInSourceRectangle = new Rectangle(wipeX, wipeY, wipeWidth, wipeHeight);
            wipeOutDestinationRectangle = new Rectangle(0, 0, wipeX, wipeHeight);
            wipeOutSourceRectangle = new Rectangle(0, 0, wipeX, wipeHeight);

            if (wipeAmount > viewport.Width)
                ChangeWipe();
        }

        private void ChangeWipe()
        {
            wipeAmount = 0.0f;
            wipeDirection--;
            if (wipeDirection < WipeDirection.Left)
                wipeDirection = WipeDirection.Down;
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;

            effect.Parameters["AmbientColor"].SetValue(0.9f);

            if (effect.Parameters["LightPosition"] != null)
                effect.Parameters["LightPosition"].SetValue(new Vector3(0, 0, -50));

            //Set up our render target
            device.SetRenderTarget(renderTarget);
            //Clear out our render target
            device.Clear(Color.Black);

            //Draw Scene
            Matrix world = Matrix.CreateRotationY(
                    MathHelper.ToRadians(45.0f *
                    (float)gameTime.TotalGameTime.TotalSeconds)) *
                    Matrix.CreateTranslation(new Vector3(0, 0, -4000));
            DrawModel(ref model, ref world, texture);

            //clear it out
            device.SetRenderTarget(null);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            Texture2D sceneTexture = renderTarget;

            //now, we can draw it for real ...
            //clear out our buffer
            device.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            //start transition
            switch (state)
            {
                case TransitionState.CrossFade:
                    {
                        //Draw the screen we are transitioning to first
                        spriteBatch.Draw(splashScreen, Vector2.Zero, Color.White);

                        //Then draw the screen we are transitioning from
                        spriteBatch.Draw(renderTarget, Vector2.Zero,
                            new Color(new Vector4(Color.White.ToVector3(), 1.0f - fadeAmount)));

                        break;
                    }
                case TransitionState.Wipe:
                    {
                        //Draw the scree`n we are transitioning from first
                        spriteBatch.Draw(renderTarget, wipeOutDestinationRectangle, wipeOutSourceRectangle, Color.White);

                        //Then draw the screen we are transitioning into
                        spriteBatch.Draw(splashScreen, wipeInDestinationRectangle, wipeInSourceRectangle, Color.White);

                        break;
                    }
                default:
                    {
                        spriteBatch.Draw(renderTarget, Vector2.Zero,
                        Color.White);
                        break;
                    }
            }

            //close our batch
            spriteBatch.End();

            base.Draw(gameTime);
        }

       private void DrawModel(ref Model m, ref Matrix world, Texture2D texture)
       {
           Matrix[] transforms = new Matrix[m.Bones.Count];
           m.CopyAbsoluteBoneTransformsTo(transforms);

           foreach (ModelMesh mesh in m.Meshes)
           {
               foreach (ModelMeshPart mp in mesh.MeshParts)
               {
                   effect.Parameters["ColorMap"].SetValue(texture);
                   effect.Parameters["Projection"].SetValue(camera.Projection);
                   effect.Parameters["View"].SetValue(camera.View);
                   effect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                   mp.Effect = effect;
               }
               mesh.Draw();
           }
       }
    }
}
