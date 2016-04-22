using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using XELibrary;

namespace TunnelVision
{
    public class Tunnel : DrawableGameComponent
    {
        private VertexPositionNormalTangentTexture[] vertices;
        private short[] indices;
        private VertexDeclaration vertexDeclaration;

        private Effect effect;
        private Texture2D colorMap;
        private Texture2D normalMap;

        public Matrix View;
        public Matrix Projection;

        private TunnelVision ourGame;

        public Tunnel(Game game)
            : base(game)
        {
            ourGame = (TunnelVision)game;
        }

        public override void Initialize()
        {
            base.Initialize();

            InitializeVertices();
            InitializeIndices();
        }

        private void InitializeVertices()
        {
            Vector3 position;
            Vector2 textureCoordinates;

            vertices = new VertexPositionNormalTangentTexture[4];

            //top left
            position = new Vector3(-100, 100, 0);
            textureCoordinates = new Vector2(0, 0);
            vertices[0] = new VertexPositionNormalTangentTexture(
                position, Vector3.Forward, Vector3.Left, textureCoordinates);

            //bottom right
            position = new Vector3(100, -100, 0);
            textureCoordinates = new Vector2(1, 1);
            vertices[1] = new VertexPositionNormalTangentTexture(
                position, Vector3.Forward, Vector3.Left, textureCoordinates);

            //bottom left
            position = new Vector3(-100, -100, 0);
            textureCoordinates = new Vector2(0, 1);
            vertices[2] = new VertexPositionNormalTangentTexture(
                position, Vector3.Forward, Vector3.Left, textureCoordinates);

            //top right
            position = new Vector3(100, 100, 0);
            textureCoordinates = new Vector2(1, 0);
            vertices[3] = new VertexPositionNormalTangentTexture(
                position, Vector3.Forward, Vector3.Left, textureCoordinates);
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

        protected override void LoadContent()
        {
            //Initialize our Vertex Declaration
            //vertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionNormalTangentTexture.VertexElements);

            effect = ourGame.Content.Load<Effect>(
                @"Effects\NormalMapping");
            colorMap = ourGame.Content.Load<Texture2D>(
                @"Textures\rockbump_color");
            normalMap = ourGame.Content.Load<Texture2D>(
                @"Textures\rockbump_normal");

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.RenderState.AlphaTestEnable = false;
            //GraphicsDevice.RenderState.AlphaBlendEnable = false;
            //GraphicsDevice.RenderState.PointSpriteEnable = false;
            //GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            //GraphicsDevice.RenderState.DepthBufferEnable = true;

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            //GraphicsDevice.VertexDeclaration = vertexDeclaration;

            Matrix world = Matrix.Identity;
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(View);
            effect.Parameters["Projection"].SetValue(Projection);
            effect.Parameters["AmbientColor"].SetValue(.05f);
            effect.Parameters["ColorMap"].SetValue(colorMap);
            effect.Parameters["NormalMap"].SetValue(normalMap);
            effect.Parameters["LightPosition"].SetValue(Vector3.Zero);
            effect.Parameters["LightDiffuseColor"].SetValue(Color.White.ToVector4());

            DrawRectangle(world * Matrix.CreateRotationY(
                MathHelper.ToRadians(-90)) *
                Matrix.CreateTranslation(60, 0, 100), effect); //right
            DrawRectangle(world * Matrix.CreateRotationY(
                MathHelper.ToRadians(90)) *
                Matrix.CreateTranslation(-60, 0, 100), effect); //left
            DrawRectangle(world * Matrix.CreateRotationX(
                MathHelper.ToRadians(90)) *
                Matrix.CreateTranslation(0, 60, 100), effect); //top
            DrawRectangle(world * Matrix.CreateRotationX(
                MathHelper.ToRadians(-90)) *
                Matrix.CreateTranslation(0, -60, 100), effect); //bottom

            base.Draw(gameTime);
        }

        private void DrawRectangle(Matrix world, Effect effect)
        {
            effect.Parameters["World"].SetValue(world);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList, vertices, 0, vertices.Length,
                    indices, 0, indices.Length / 3);
            }
        }


    }
}