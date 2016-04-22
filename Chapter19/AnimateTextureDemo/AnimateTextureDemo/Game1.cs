using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using XELibrary;

namespace AnimateTextureDemo
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

        private VertexPositionNormalTangentTexture[] vertices;
        private VertexBuffer vertexBuffer;
        private VertexDeclaration vertexDeclaration;
        private short[] indices;

        private Effect effect;
        private Texture2D colorMap;
        private Vector3 LightPosition;
        private Vector2 offset = Vector2.Zero;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

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
            InitializeVertices();
            InitializeIndices();

            //Initialize our Vertex Declaration
            vertexDeclaration = new VertexDeclaration(VertexPositionNormalTangentTexture.VertexElements);

            LightPosition = new Vector3(0, 0, -300);

            base.Initialize();
        }


        private void InitializeVertices()
        {
            Vector3 position;
            Vector2 textureCoordinates;

            vertices = new VertexPositionNormalTangentTexture[4];

            //top left
            position = new Vector3(-1, 1, 0);
            textureCoordinates = new Vector2(0, 0);
            vertices[0] = new VertexPositionNormalTangentTexture(position,
                Vector3.Forward, Vector3.Left, textureCoordinates);

            //bottom right
            position = new Vector3(1, -1, 0);
            textureCoordinates = new Vector2(1, 1);
            vertices[1] = new VertexPositionNormalTangentTexture(position,
                Vector3.Forward, Vector3.Left, textureCoordinates);

            //bottom left
            position = new Vector3(-1, -1, 0);
            textureCoordinates = new Vector2(0, 1);
            vertices[2] = new VertexPositionNormalTangentTexture(position,
                Vector3.Forward, Vector3.Left, textureCoordinates);

            //top right
            position = new Vector3(1, 1, 0);
            textureCoordinates = new Vector2(1, 0);
            vertices[3] = new VertexPositionNormalTangentTexture(position,
                Vector3.Forward, Vector3.Left, textureCoordinates);

            vertexBuffer = new VertexBuffer(graphics.GraphicsDevice,
                typeof(VertexPositionNormalTangentTexture),
                vertices.Length,
                BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormalTangentTexture>(vertices);
        }

        private void InitializeIndices()
        {
            //six vertices make up two triangles, which make up our rectangle
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

            colorMap = Content.Load<Texture2D>(@"Textures\example");
            effect = Content.Load<Effect>(@"Effects\AnimateTexture");
            effect.Parameters["AmbientColor"].SetValue(.05f);
            effect.Parameters["ColorMap"].SetValue(colorMap);
            effect.Parameters["LightPosition"].SetValue(LightPosition);

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

            offset.X += (float)gameTime.ElapsedGameTime.TotalSeconds * 0.0333f;
            offset.Y += (float)gameTime.ElapsedGameTime.TotalSeconds * 0.0033f;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            //graphics.GraphicsDevice.RenderState.CullMode = CullMode.None;
            //graphics.GraphicsDevice.VertexDeclaration = vertexDeclaration;


            Matrix world = Matrix.CreateScale(100.0f) *
                Matrix.CreateTranslation(new Vector3(0, 0, -250));
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["Offset"].SetValue(offset);


            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphics.GraphicsDevice.DrawUserIndexedPrimitives
                    <VertexPositionNormalTangentTexture>(
                    PrimitiveType.TriangleList, vertices, 0, vertices.Length,
                    indices, 0, indices.Length / 3);
            }

            base.Draw(gameTime);
        }
    }
}
