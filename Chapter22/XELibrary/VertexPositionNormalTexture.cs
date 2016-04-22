using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XELibrary
{
    public struct VertexPositionNormalTangentTexture : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector2 TextureCoordinate;

        public VertexPositionNormalTangentTexture(
            Vector3 Position,
            Vector3 Normal,
            Vector3 Tangent,
            Vector2 TextureCoordinate)
        {
            this.Position = Position;
            this.Normal = Normal;
            this.Tangent = Tangent;
            this.TextureCoordinate = TextureCoordinate;
        }

        public static int SizeInBytes = 11 * sizeof(float);

        public static VertexElement[] VertexElements =
             {
                 new VertexElement(
                     0, VertexElementFormat.Vector3,
                     VertexElementUsage.Position, 0),
                 new VertexElement(sizeof(float)*3,
                     VertexElementFormat.Vector3,
                     VertexElementUsage.Normal, 0),
                 new VertexElement(sizeof(float)*6,
                     VertexElementFormat.Vector3,
                     VertexElementUsage.Tangent, 0),
                 new VertexElement(sizeof(float)*9,
                     VertexElementFormat.Vector2,
                     VertexElementUsage.TextureCoordinate, 0)
             };

        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return new VertexDeclaration(VertexElements);
            }
        }
    }
}
