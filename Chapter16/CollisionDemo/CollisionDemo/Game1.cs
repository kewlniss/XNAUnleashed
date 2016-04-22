using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using XELibrary;

namespace CollisionDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Model sphere;

        private PhysicalObject[] spheres = new PhysicalObject[5];
        private float e; //coefficient of restitution

        private InputHandler input;
        private FirstPersonCamera camera;
        private FPS fps;

        private Vector3 friction;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            input = new InputHandler(this);
            Components.Add(input);
            camera = new FirstPersonCamera(this);
            Components.Add(camera);
            fps = new FPS(this, false, true);
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
            for (int i = 0; i < spheres.Length; i++)
                spheres[i] = new PhysicalObject();

            InitializeValues();

            base.Initialize();
        }

        private void InitializeValues()
        {
            friction = new Vector3(-0.025f);

            e = 0.95f;

            spheres[0].Position = new Vector3(-90.0f, 0, -300.0f);
            spheres[0].Velocity = new Vector3(60.0f, 0, 0);
            spheres[0].Mass = 1.0f;
            spheres[0].Color = Color.Silver;

            /*
            spheres[0].Position = new Vector3(-25.0f, 5.0f, -430);
            spheres[0].Velocity = new Vector3(70.0f, -8.0f, 70);
            spheres[0].Mass = 6.0f;
            spheres[0].Color = Color.Silver;
            */

            for (int i = 1; i < spheres.Length; i++)
            {
                spheres[i].Position = new Vector3(25.0f + (i * 25), 0, -300);
                spheres[i].Velocity = new Vector3(-5.0f, 0, 0);
                spheres[i].Mass = 4.0f;
                spheres[i].Color = Color.Red;
            }

            spheres[spheres.Length - 1].Velocity = Vector3.Zero;
            spheres[spheres.Length - 1].Mass = 6.0f;
            spheres[spheres.Length - 1].Color = Color.Black;
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            sphere = Content.Load<Model>(@"Models\sphere0");

            fps.Load(spriteBatch);
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
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (input.KeyboardState.WasKeyPressed(Keys.Enter))
                InitializeValues();

            for (int i = 0; i < spheres.Length; i++)
            {
                spheres[i].World = Matrix.CreateScale(spheres[i].Mass) *
                    Matrix.CreateTranslation(spheres[i].Position);

                Vector3 trans, scale;
                Matrix rot;
                MatrixDecompose(spheres[i].World, out trans, out scale, out rot);
                spheres[i].Radius = scale.Length();

                ApplyFriction(ref spheres[i].Velocity);
            }

            for (int a = 0; a < spheres.Length; a++)
            {
                for (int b = a + 1; b < spheres.Length; b++)
                {
                    if (a == b)
                        continue; //don’t check against yourself

                    float distance = (spheres[a].Position -
                                      spheres[b].Position).Length();
                    float tmp = 1.0f / (spheres[a].Mass + spheres[b].Mass);

                    float collisionDistance = distance - (spheres[a].Radius +
                                                          spheres[b].Radius);

                    if (collisionDistance <= 0)
                    {
                        Vector3 velocity1 = (
                            (e + 1.0f) * spheres[b].Mass * spheres[b].Velocity +
                        spheres[a].Velocity * (spheres[a].Mass - (e * spheres[b].Mass))
                        ) * tmp;

                        Vector3 velocity2 = (
                            (e + 1.0f) * spheres[a].Mass * spheres[a].Velocity +
                        spheres[b].Velocity * (spheres[b].Mass - (e * spheres[a].Mass))
                        ) * tmp;

                        spheres[a].Velocity = velocity1;
                        spheres[b].Velocity = velocity2;
                    }
                }
                spheres[a].Position = spheres[a].Position +
                    (elapsedTime * (spheres[a].Velocity));
            }
            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            for (int i = 0; i < spheres.Length; i++)
                DrawModel(ref sphere, ref spheres[i].World, spheres[i].Color);


            base.Draw(gameTime);
        }

        private void DrawModel(ref Model m, ref Matrix world, Color color)
        {
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();

                    be.AmbientLightColor = color.ToVector3();
                    be.Projection = camera.Projection;
                    be.View = camera.View;
                    be.World = world * mesh.ParentBone.Transform;
                }

                mesh.Draw();
            }
        }

        private void ApplyFriction(ref Vector3 velocity)
        {
            if (velocity.X < 0)
                velocity.X -= friction.X;
            if (velocity.X > 0)
                velocity.X += friction.X;

            if (velocity.Y < 0)
                velocity.Y -= friction.Y;
            if (velocity.Y > 0)
                velocity.Y += friction.Y;

            if (velocity.Z < 0)
                velocity.Z -= friction.Z;
            if (velocity.Z > 0)
                velocity.Z += friction.Z;
        }

        public void MatrixDecompose(Matrix mat,
                         out Vector3 trans,
                         out Vector3 scale,
                         out Matrix rot)
        {
            trans = Vector3.Zero;
            scale = Vector3.Zero;
            rot = Matrix.Identity;

            Vector3[] cols = new Vector3[]{
                    new Vector3(mat.M11,mat.M12,mat.M13),
                    new Vector3(mat.M21,mat.M22,mat.M23),
                    new Vector3(mat.M31,mat.M32,mat.M33)  
                };

            scale.X = cols[0].Length();
            scale.Y = cols[1].Length();
            scale.Z = cols[2].Length();


            trans.X = mat.M41 / (scale.X == 0 ? 1 : scale.X);
            trans.Y = mat.M42 / (scale.Y == 0 ? 1 : scale.Y);
            trans.Z = mat.M43 / (scale.Z == 0 ? 1 : scale.Z);

            if (scale.X != 0)
            {
                cols[0].X /= scale.X;
                cols[0].Y /= scale.X;
                cols[0].Z /= scale.X;
            }
            if (scale.Y != 0)
            {
                cols[1].X /= scale.Y;
                cols[1].Y /= scale.Y;
                cols[1].Z /= scale.Y;
            }
            if (scale.Z != 0)
            {
                cols[2].X /= scale.Z;
                cols[2].Y /= scale.Z;
                cols[2].Z /= scale.Z;
            }

            rot.M11 = cols[0].X;
            rot.M12 = cols[0].Y;
            rot.M13 = cols[0].Z;
            rot.M14 = 0;
            rot.M41 = 0;
            rot.M21 = cols[1].X;
            rot.M22 = cols[1].Y;
            rot.M23 = cols[1].Z;
            rot.M24 = 0;
            rot.M42 = 0;
            rot.M31 = cols[2].X;
            rot.M32 = cols[2].Y;
            rot.M33 = cols[2].Z;
            rot.M34 = 0;
            rot.M43 = 0;
            rot.M44 = 1;
        }

    }

    class PhysicalObject
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public float Mass;
        public float Radius;
        public Matrix World;
        public Color Color;
    }

}
