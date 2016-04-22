using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using XELibrary;

namespace AIDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Model sphere;
        private Skybox skybox;

        private InputHandler input;
        private Camera camera;

        private const int MaxEnemies = 10;
        private Player player;
        private Enemy[] enemies = new Enemy[MaxEnemies];

        private const float ArenaSize = 500;

        private bool restrictToXY = true;

        private float playerMoveUnit = 25;

        private float moveUnit = 20;

        private Random rand = new Random();

        private const float SeekRadius = 75.0f;
        private const float EvadeRadius = 75.0f;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            input = new InputHandler(this, true);
            Components.Add(input);
            camera = new Camera(this);
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
            player = new Player();
            player.Position = new Vector3(-100, 0, -300);
            player.Velocity = Vector3.Zero;
            player.World = Matrix.CreateTranslation(player.Position);
            player.Color = Color.Black;

            for (int i = 0; i < MaxEnemies; i++)
            {
                enemies[i] = new Enemy();
                enemies[i].Position = new Vector3((i * 50) + 50, (i * 25) - 50, -300);
                enemies[i].Velocity = Vector3.Zero;
                enemies[i].World = Matrix.CreateTranslation(enemies[i].Position);
                enemies[i].ChangeDirectionTimer = 0;
                enemies[i].RandomVelocity = Vector3.Left;
                enemies[i].RandomSeconds = (i + 2) / 2;
                enemies[i].State = Enemy.AIState.Search;
                //alternate every other enemy to either attack or evade (based on health)
                if (i % 2 == 0)
                    enemies[i].Health = 1;
                else
                    enemies[i].Health = 10;
            }


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

            sphere = Content.Load<Model>(@"Models\sphere0");
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdatePlayer();
            player.Velocity *= elapsedTime;
            player.Position += player.Velocity;
            KeepWithinBounds(ref player.Position, ref player.Velocity);
            player.World = Matrix.CreateTranslation(player.Position);

            foreach (Enemy enemy in enemies)
            {
                switch (enemy.State)
                {
                    case Enemy.AIState.Search:
                        {
                            MoveRandomly(enemy, gameTime);
                            break;
                        }
                    case Enemy.AIState.Attack:
                        {
                            TrackPlayer(enemy);
                            break;
                        }
                    case Enemy.AIState.Retreat:
                        {
                            EvadePlayer(enemy);
                            break;
                        }
                    default:
                        {
                            throw (new Exception("Unknown State: " +
                                enemy.State.ToString()));
                        }
                }

                enemy.Velocity *= elapsedTime;
                enemy.Position += enemy.Velocity;
                KeepWithinBounds(ref enemy.Position, ref enemy.Velocity);
                enemy.World = Matrix.CreateTranslation(enemy.Position);

                //reset player or enemy if collided
                if ((enemy.Position - player.Position).Length() <
                    (sphere.Meshes[0].BoundingSphere.Radius * 2))
                {
                    if (enemy.State == Enemy.AIState.Attack)
                        player.Position.X = player.Position.Y = 0;
                    else
                        enemy.Position.X = enemy.Position.Y = 0;
                }
            }

            base.Update(gameTime);
        }

        private void UpdatePlayer()
        {
            player.Velocity = Vector3.Zero;

            if (input.KeyboardState.IsHoldingKey(Keys.W) ||
                input.GamePads[0].ThumbSticks.Left.Y > 0)
            {
                player.Velocity.Y++;
            }
            else if (input.KeyboardState.IsHoldingKey(Keys.S) ||
                input.GamePads[0].ThumbSticks.Left.Y < 0)
            {
                player.Velocity.Y--;
            }

            if (input.KeyboardState.IsHoldingKey(Keys.D) ||
                input.GamePads[0].ThumbSticks.Left.X > 0)
            {
                player.Velocity.X++;
            }
            else if (input.KeyboardState.IsHoldingKey(Keys.A) ||
                input.GamePads[0].ThumbSticks.Left.X < 0)
            {
                player.Velocity.X--;
            }

            //restrict to 2D?
            if (!restrictToXY)
            {
                if (input.KeyboardState.IsHoldingKey(Keys.RightShift) ||
                    input.GamePads[0].Triggers.Right > 0)
                {
                    player.Velocity.Z--;
                }
                else if (input.KeyboardState.IsHoldingKey(Keys.RightControl) ||
                    input.GamePads[0].Triggers.Left > 0)
                {
                    player.Velocity.Z++;
                }
            }

            //Normalize our vector so we don’t go faster
            //when heading in multiple directions
            if (player.Velocity.LengthSquared() != 0)
                player.Velocity.Normalize();

            player.Velocity *= playerMoveUnit;
        }

        private void KeepWithinBounds(ref Vector3 position, ref Vector3 velocity)
        {
            if ((position.X < -ArenaSize) || (position.X > ArenaSize))
                velocity.X = -velocity.X;
            if ((position.Y < -ArenaSize) || (position.Y > ArenaSize))
                velocity.Y = -velocity.Y;
            if ((position.Z < -ArenaSize) || (position.Z > ArenaSize))
                velocity.Z = -velocity.Z;
            position += velocity;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            skybox.Draw(camera.View, camera.Projection, Matrix.CreateScale(ArenaSize));
            //Draw player
            DrawModel(ref sphere, ref player.World, player.Color);

            //Draw enemies
            for (int i = 0; i < MaxEnemies; i++)
            {
                Enemy enemy = enemies[i];
                DrawModel(ref sphere, ref enemy.World, enemy.Color);
            }

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

        private void TrackPlayerStraightLine(Enemy enemy)
        {
            if (player.Position.X > enemy.Position.X)
                enemy.Velocity.X = moveUnit;
            else if (player.Position.X < enemy.Position.X)
                enemy.Velocity.X = -moveUnit;
            else
                enemy.Velocity.X = 0;

            if (player.Position.Y > enemy.Position.Y)
                enemy.Velocity.Y = moveUnit;
            else if (player.Position.Y < enemy.Position.Y)
                enemy.Velocity.Y = -moveUnit;
            else
                enemy.Velocity.Y = 0;

            //restrict to 2D?
            if (!restrictToXY)
            {
                if (player.Position.Z > enemy.Position.Z)
                    enemy.Velocity.Z = moveUnit;
                else if (player.Position.Z < enemy.Position.Z)
                    enemy.Velocity.Z = -moveUnit;
                else
                    enemy.Velocity.Z = 0;
            }

            enemy.Color = Color.Red;

            float distance = (enemy.Position - player.Position).Length();
            if (distance > SeekRadius * 1.25f)
                enemy.State = Enemy.AIState.Search;
        }

        private void TrackPlayer(Enemy enemy)
        {
            Vector3 tv = player.Position - enemy.Position;
            tv.Normalize();

            enemy.Velocity = tv * moveUnit;

            enemy.Color = Color.Red;

            float distance = (enemy.Position - player.Position).Length();
            if (distance > SeekRadius * 1.25f)
                enemy.State = Enemy.AIState.Search;
        }

        private void EvadePlayer(Enemy enemy)
        {
            Vector3 tv = enemy.Position - player.Position;
            float distance = tv.Length();
            tv.Normalize();

            enemy.Velocity = tv * moveUnit;

            enemy.Color = Color.Navy;

            if (distance > EvadeRadius * 1.25f)
                enemy.State = Enemy.AIState.Search;
        }

        private void MoveRandomly(Enemy enemy, GameTime gameTime)
        {
            if (enemy.ChangeDirectionTimer == 0)
                enemy.ChangeDirectionTimer =
                    (float)gameTime.TotalGameTime.TotalMilliseconds;

            //has the appropriate amount of time passed?
            if (gameTime.TotalGameTime.TotalMilliseconds >
                enemy.ChangeDirectionTimer + enemy.RandomSeconds * 1000)
            {
                enemy.RandomVelocity = Vector3.Zero;
                enemy.RandomVelocity.X = rand.Next(-1, 2);
                enemy.RandomVelocity.Y = rand.Next(-1, 2);
                //restrict to 2D?
                if (!restrictToXY)
                    enemy.RandomVelocity.Z = rand.Next(-1, 2);

                enemy.ChangeDirectionTimer = 0;
            }

            enemy.Velocity = enemy.RandomVelocity;

            enemy.Velocity *= moveUnit;

            enemy.Color = Color.Orange;

            float distance = (player.Position - enemy.Position).Length();

            if (distance < EvadeRadius)
                if (enemy.Health < 5)
                    enemy.State = Enemy.AIState.Retreat;

            if (distance < SeekRadius)
                if (enemy.Health >= 5)
                    enemy.State = Enemy.AIState.Attack;
        }

    }

    class Player
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public Matrix World;
        public Color Color;
    }

    class Enemy
    {
        public enum AIState { Attack, Retreat, Search }
        
        public AIState State;
        public Vector3 Position;
        public Vector3 Velocity;
        public Matrix World;
        public Color Color;
        public float ChangeDirectionTimer;
        public Vector3 RandomVelocity;
        public int RandomSeconds;
        public int Health;
    }

}
