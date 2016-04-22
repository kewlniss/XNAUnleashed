using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

using XELibrary;

namespace ParticleSystemDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private FirstPersonCamera camera;
        private InputHandler input;
        private FPS fps;

        private Model model;
        private Texture2D texture;
        private Effect effect;
        private Skybox skybox;

        //private Rain rain;
        //private Bubbles bubbles;
        //private LaserShield laserShield;
        private LaserScanner laserScanner;
        //private PosionGas gas;
        //private Colorful colorful;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            input = new InputHandler(this, true);
            Components.Add(input);

            camera = new FirstPersonCamera(this);
            Components.Add(camera);

            fps = new FPS(this, false, true);
            Components.Add(fps);

            skybox = new Skybox(this);
            Components.Add(skybox);

            //rain = new Rain(this);
            //Components.Add(rain);

            //bubbles = new Bubbles(this);
            //Components.Add(bubbles);

            //laserShield = new LaserShield(this);
            //Components.Add(laserShield);

            laserScanner = new LaserScanner(this);
            Components.Add(laserScanner);

            //gas = new PosionGas(this);
            //Components.Add(gas);

            //colorful = new Colorful(this);
            //Components.Add(colorful);
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

            model = Content.Load<Model>(@"Models\asteroid1");
            texture = Content.Load<Texture2D>(@"Textures\asteroid1");
            effect = Content.Load<Effect>(@"Effects\AmbientTexture");
            effect.Parameters["AmbientColor"].SetValue(
                Color.WhiteSmoke.ToVector4());

            //skybox = Content.Load<Skybox>(@"Skyboxes\skybox2");
            skybox.Load(@"Skyboxes\skybox2");

            base.LoadContent();
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

            if (input.ButtonHandler.WasButtonPressed(0, Buttons.A) ||
                input.KeyboardState.WasKeyPressed(Keys.Space))
            {
                laserScanner.ResetSystem();
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

            //graphics.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            //graphics.GraphicsDevice.RenderState.PointSpriteEnable = false;
            //graphics.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;

            GraphicsDevice.BlendState = BlendState.Additive;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            laserScanner.View = camera.View;
            laserScanner.Projection = camera.Projection;

            skybox.Draw(camera.View, camera.Projection, Matrix.CreateScale(5000.0f));

            Matrix world = Matrix.CreateRotationY(
                MathHelper.ToRadians(45.0f * (float)gameTime.TotalGameTime.TotalSeconds)) *
                Matrix.CreateTranslation(new Vector3(0, 0, -4000));
            DrawModel(ref model, ref world, texture);

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
                    if (texture != null)
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
