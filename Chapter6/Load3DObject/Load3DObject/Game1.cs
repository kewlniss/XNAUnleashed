using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using XELibrary;

namespace Load3DObject
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private FPS fps;
        private FirstPersonCamera camera;
        private InputHandler input;

        private Model model;

        private Texture2D originalAsteroid;
        private Texture2D greyAsteroid;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            input = new InputHandler(this);
            Components.Add(input);
            camera = new FirstPersonCamera(this);
            Components.Add(camera);

#if DEBUG
            //draw 60 fps and update as often as possible
            fps = new FPS(this, true, false);
#else
            fps = new FPS(this, true, false);
#endif
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

            model = Content.Load<Model>(@"Models\asteroid1");

            originalAsteroid = Content.Load<Texture2D>(@"Textures\asteroid1");
            greyAsteroid = Content.Load<Texture2D>(@"Textures\asteroid1-grey");
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

            Matrix world = Matrix.CreateRotationY(MathHelper.ToRadians(
                    270.0f * (float)gameTime.TotalGameTime.TotalSeconds)) *
                Matrix.CreateTranslation(new Vector3(0, 0, -4000));
            DrawModel(ref model, ref world, greyAsteroid);

            world = Matrix.CreateRotationY(MathHelper.ToRadians(
                    45.0f * (float)gameTime.TotalGameTime.TotalSeconds)) *
                Matrix.CreateRotationZ(MathHelper.ToRadians(
                    45.0f * (float)gameTime.TotalGameTime.TotalSeconds)) *
                Matrix.CreateTranslation(new Vector3(0, 0, 4000));
            DrawModel(ref model, ref world, originalAsteroid);


            base.Draw(gameTime);
        }

        private void DrawModel(ref Model m, ref Matrix world, Texture2D texture)
        {
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    if (texture != null)
                        be.Texture = texture;
                    be.Projection = camera.Projection;
                    be.View = camera.View;
                    be.World = world * transforms[mesh.ParentBone.Index]; //mesh.ParentBone.Transform;
                }

                mesh.Draw();
            }
        }

    }
}
