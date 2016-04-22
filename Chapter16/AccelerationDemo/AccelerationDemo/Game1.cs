using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using XELibrary;

namespace AccelerationDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private PhysicalObject sphere = new PhysicalObject();
        private InputHandler input;
        private FirstPersonCamera camera;
        private Model model;
        private Skybox skybox;
        private float maxSpeed = 163.0f;
        private float maxReverseSpeed = 25.0f;
        private float constantAcceleration = .5f;
        private float constantDeceleration = .85f;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            input = new InputHandler(this);
            Components.Add(input);
            camera = new FirstPersonCamera(this);
            Components.Add(camera);
            skybox = new Skybox(this);
            Components.Add(skybox);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            InitializeValues();

            base.Initialize();
        }

        private void InitializeValues()
        {
            sphere.Position = new Vector3(-15.0f, 0, -500);
            sphere.Velocity = Vector3.Zero;
            sphere.Acceleration = Vector3.Zero;
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>(@"Models\sphere0");
            skybox.Load(@"Skyboxes\skybox2");
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
            if (input.KeyboardState.WasKeyPressed(Keys.Enter))
                InitializeValues();

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //increase acceleration
            if (input.KeyboardState.IsHoldingKey(Keys.Space))
            {
                sphere.Acceleration.X += (constantAcceleration * elapsed);
            }
            //decrease acceleration (brake)
            else if (input.KeyboardState.IsHoldingKey(Keys.B))
            {
                sphere.Acceleration.X -= (constantDeceleration * elapsed);
            }
            else //coast
            {
                sphere.Acceleration.X = 0;
            }

            sphere.Velocity.X += sphere.Acceleration.X;

            if (sphere.Velocity.X > maxSpeed)
                sphere.Velocity.X = maxSpeed;
            if (sphere.Velocity.X < -maxReverseSpeed)
                sphere.Velocity.X = -maxReverseSpeed;

            sphere.Position = sphere.Position + (elapsed * sphere.Velocity);

            Window.Title = "Acceleration: " + sphere.Acceleration.X.ToString() + " Position: " + sphere.Position.X.ToString() + " Velocity: " + sphere.Velocity.X.ToString();


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            skybox.Draw(camera.View, camera.Projection, Matrix.CreateScale(200.0f));

            Matrix world = Matrix.CreateScale(10.0f) *
                           Matrix.CreateTranslation(sphere.Position);
            DrawModel(ref model, ref world);

            base.Draw(gameTime);
        }

        private void DrawModel(ref Model m, ref Matrix world)
        {
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    be.AmbientLightColor = Color.Red.ToVector3();

                    be.Projection = camera.Projection;
                    be.View = camera.View;
                    be.World = world * mesh.ParentBone.Transform;
                }
                mesh.Draw();
            }
        }
    }

    class PhysicalObject
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Acceleration;
    }

}
