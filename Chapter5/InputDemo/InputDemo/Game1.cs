using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using XELibrary;

namespace InputDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private VertexPositionNormalTexture[] vertices;

        private Texture2D texture;

        private short[] indices;

        private FPS fps;

        private BasicEffect effect;

        private FirstPersonCamera camera;

        private InputHandler input;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            input = new InputHandler(this);
            Components.Add(input);

            camera = new FirstPersonCamera(this);
            Components.Add(camera);

#if DEBUG
            fps = new FPS(this);
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

        private void InitializeVertices()
        {
            Vector3 position;
            Vector2 textureCoordinates;

            vertices = new VertexPositionNormalTexture[4];

            //top left
            position = new Vector3(-1, 1, 0);
            textureCoordinates = new Vector2(0, 0);
            vertices[0] = new VertexPositionNormalTexture(position, Vector3.Forward,
                textureCoordinates);

            //bottom right
            position = new Vector3(1, -1, 0);
            textureCoordinates = new Vector2(1, 1);
            vertices[1] = new VertexPositionNormalTexture(position, Vector3.Forward,
                textureCoordinates);

            //bottom left
            position = new Vector3(-1, -1, 0);
            textureCoordinates = new Vector2(0, 1);
            vertices[2] = new VertexPositionNormalTexture(position, Vector3.Forward,
                textureCoordinates);

            //top right
            position = new Vector3(1, 1, 0);
            textureCoordinates = new Vector2(1, 0);
            vertices[3] = new VertexPositionNormalTexture(position, Vector3.Forward,
                textureCoordinates);

        }

        private void InitializeIndices()
        {
            //6 vertices make up 2 triangles which make up our rectangle
            indices = new short[6];

            //triangle 1 (bottom portion)
            indices[0] = 0; // top left
            indices[1] = 1; // bottom right
            indices[2] = 2; // bottom left

            //triangle 2 (top portion)
            indices[3] = 0; // top left
            indices[4] = 3; // top right
            indices[5] = 1; // bottom right
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
            InitializeVertices();

            InitializeIndices();

            texture = Content.Load<Texture2D>("texture");

            effect = new BasicEffect(graphics.GraphicsDevice);
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
            if (input.GamePads[0].IsConnected)
            {
                GamePad.SetVibration(PlayerIndex.One, input.GamePads[0].Triggers.Left,
                    input.GamePads[0].Triggers.Right);

                this.Window.Title = "left: " +
                    input.GamePads[0].Triggers.Left.ToString() + "; right: " +
                    input.GamePads[0].Triggers.Right.ToString();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            effect.Projection = camera.Projection;
            effect.View = camera.View;

            effect.EnableDefaultLighting();

            effect.TextureEnabled = true;
            effect.Texture = texture;

            Matrix world = Matrix.CreateTranslation(new Vector3(3.0f, 0, -10.0f));
            DrawRectangle(ref world);

            world = Matrix.CreateScale(0.75f) *
               Matrix.CreateRotationX(MathHelper.ToRadians(15.0f)) *
               Matrix.CreateRotationY(MathHelper.ToRadians(30.0f)) *
               Matrix.CreateTranslation(new Vector3(-3.0f, -1.0f, -5.0f));

            DrawRectangle(ref world);

            world = Matrix.CreateTranslation(new Vector3(8.0f, 0, -10.0f));
            DrawRectangle(ref world);

            world = Matrix.CreateTranslation(new Vector3(8.0f, 0, -6.0f));
            DrawRectangle(ref world);

            world = Matrix.CreateRotationY(MathHelper.ToRadians(180f)) *
                Matrix.CreateTranslation(new Vector3(3.0f, 0, 10.0f));

            DrawRectangle(ref world);

            base.Draw(gameTime);
        }

        private void DrawRectangle(ref Matrix world)
        {
            effect.World = world;

            //As we are doing a basic effect, there is no need to loop
            //basic effect will only have one pass on the technique
            effect.CurrentTechnique.Passes[0].Apply();

            graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList, vertices, 0, vertices.Length,
                    indices, 0, indices.Length / 3);
        }
    }
}
