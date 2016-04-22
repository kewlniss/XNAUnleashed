using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using XELibrary;

namespace AdvancedTexturingDemo
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

        private Effect ambientEffect;
        private Effect currentEffect;
        private Texture2D colorMap;

        private Vector3 LightPosition;
        private Effect directionalEffect;

        private Effect normalEffect;
        private Texture2D normalMap;

        private Effect parallaxEffect;
        private Texture2D depthMap;

        private Effect reliefEffect;
        private Texture2D reliefMap;

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

            LightPosition = new Vector3(0, 0, -500);

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

            colorMap = Content.Load<Texture2D>(@"Textures\rockbump_color");
            ambientEffect = Content.Load<Effect>(@"Effects\AmbientTexture");

            directionalEffect = Content.Load<Effect>(@"Effects\DirectionalLight");

            normalMap = Content.Load<Texture2D>(@"Textures\rockbump_normal");
            normalEffect = Content.Load<Effect>(@"Effects\NormalMapping");

            depthMap = Content.Load<Texture2D>(@"Textures\rockbump_depth");
            parallaxEffect = Content.Load<Effect>(@"Effects\ParallaxMapping");

            reliefMap = Content.Load<Texture2D>(@"Textures\rockbump_relief");
            reliefEffect = Content.Load<Effect>(@"Effects\ReliefMapping");

            currentEffect = directionalEffect;

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

            if (input.KeyboardState.IsHoldingKey(Keys.Z))
                LightPosition.Y++;
            if (input.KeyboardState.IsHoldingKey(Keys.X))
                LightPosition.Y--;

            Window.Title = "LightPosition: " + LightPosition.ToString();

            if (input.KeyboardState.WasKeyPressed(Keys.D1)||
                input.ButtonHandler.WasButtonPressed(0, Buttons.A))
                currentEffect = ambientEffect;
            if (input.KeyboardState.WasKeyPressed(Keys.D2) ||
                input.ButtonHandler.WasButtonPressed(0, Buttons.B))
                currentEffect = directionalEffect;
            if (input.KeyboardState.WasKeyPressed(Keys.D3) ||
                input.ButtonHandler.WasButtonPressed(0, Buttons.X))
                currentEffect = normalEffect;
            if (input.KeyboardState.WasKeyPressed(Keys.D4) ||
                input.ButtonHandler.WasButtonPressed(0, Buttons.Y))
                currentEffect = parallaxEffect;
            if (input.KeyboardState.WasKeyPressed(Keys.D5) ||
                input.ButtonHandler.WasButtonPressed(0, Buttons.Start))
                currentEffect = reliefEffect;

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

            Matrix world = Matrix.CreateScale(100.0f) * Matrix.CreateTranslation(
                new Vector3(0, 0, -450));
            currentEffect.Parameters["World"].SetValue(world);
            currentEffect.Parameters["View"].SetValue(camera.View);
            currentEffect.Parameters["Projection"].SetValue(camera.Projection);

            if (currentEffect == ambientEffect)
                currentEffect.Parameters["AmbientColor"].SetValue(0.9f);
            else
                currentEffect.Parameters["AmbientColor"].SetValue(.05f);

            currentEffect.Parameters["ColorMap"].SetValue(colorMap);

            if (currentEffect.Parameters["LightPosition"] != null)
                currentEffect.Parameters["LightPosition"].SetValue(LightPosition);

            if (currentEffect.Parameters["LightDiffuseColor"] != null)
                currentEffect.Parameters["LightDiffuseColor"].SetValue(
                    Color.White.ToVector4());

            if (currentEffect.Parameters["NormalMap"] != null)
                currentEffect.Parameters["NormalMap"].SetValue(normalMap);

            if (currentEffect.Parameters["DepthMap"] != null)
                currentEffect.Parameters["DepthMap"].SetValue(depthMap);
            if (currentEffect.Parameters["ScaleAmount"] != null)
                currentEffect.Parameters["ScaleAmount"].SetValue(
                    new Vector2(0.03f, -0.025f));
            if (currentEffect.Parameters["CameraPosition"] != null)
                currentEffect.Parameters["CameraPosition"].SetValue(camera.Position);

            if (currentEffect.Parameters["ReliefMap"] != null)
                currentEffect.Parameters["ReliefMap"].SetValue(reliefMap);

            foreach (EffectPass pass in currentEffect.CurrentTechnique.Passes)
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
