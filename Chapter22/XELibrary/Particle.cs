using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XELibrary
{
    public class Particle
    {
        private Vector3 velocity;
        private Vector3 acceleration;
        private float lifetime;
        private Vector3 externalForce;

        internal float Age;
        internal bool IsAlive;
        //internal VertexPointSprite Vertex;
        internal VertexPositionColor Vertex;
        internal float ColorChangeRate;
        internal float CurrentColorTime;
        internal int CurrentColorIndex;

        public Particle()
        {
            Age = 0.0f;
            //Vertex = new VertexPointSprite();
            Vertex = new VertexPositionColor();
            CurrentColorIndex = 0;
            CurrentColorTime = 0;
        }

        internal void Update(float elapsedTime)
        {
            Age += elapsedTime;
            CurrentColorTime += elapsedTime;

            if (Age >= lifetime)
                IsAlive = false;
            else
            {
                velocity += acceleration;
                velocity -= externalForce;
                Vertex.Position += velocity * elapsedTime;
            }
        }

        internal void Initialize(ParticleSystemSettings settings)
        {
            Initialize(settings, true);
        }

        internal void Initialize(ParticleSystemSettings settings, bool makeAlive)
        {
            Age = 0;
            IsAlive = makeAlive;

            Vector3 minPosition = (settings.EmitPosition - (settings.EmitRange * .5f));
            Vector3 maxPosition = (settings.EmitPosition + (settings.EmitRange * .5f));

            Vector3 position = Utility.GetRandomVector3(minPosition, maxPosition);

            Vertex.Position = position;

            if (settings.EmitRadius != Vector2.Zero)
            {
                float angle = Utility.GetRandomFloat(0, MathHelper.TwoPi);

                Vertex.Position = new Vector3(
                    position.X + (float)Math.Sin(angle) * settings.EmitRadius.X,
                    position.Y,
                    position.Z + (float)Math.Cos(angle) * settings.EmitRadius.Y);
            }

            velocity = Utility.GetRandomVector3(
                settings.MinimumVelocity, settings.MaximumVelocity);

            acceleration = Utility.GetRandomVector3(
                settings.MinimumAcceleration, settings.MaximumAcceleration);

            lifetime = Utility.GetRandomFloat(
                settings.MinimumLifetime, settings.MaximumLifetime);

            if (settings.DisplayColorsInOrder)
            {
                Vertex.Color = settings.Colors[0];
                ColorChangeRate = lifetime / settings.Colors.Length;
            }
            else
            {
                Vertex.Color =
                    settings.Colors[Utility.GetRandomInt(0, settings.Colors.Length)];
            }

            //Vertex.PointSize = Utility.GetRandomFloat(
            //    settings.MinimumSize, settings.MaximumSize);

            externalForce = settings.ExternalForce;
        }

        internal void SetColor(Color color)
        {
            Vertex.Color = color;
        }
    }

    public struct VertexPointSprite : IVertexType
    {
        public Vector3 Position;
        public float PointSize;
        public Color Color;

        public VertexPointSprite(
            Vector3 Position,
            Color Color,
            float PointSize)
        {
            this.Position = Position;
            this.Color = Color;
            this.PointSize = PointSize;
        }

        public static int SizeInBytes = 8 * sizeof(float);

        public static VertexElement[] VertexElements =
         {
             new VertexElement(0, VertexElementFormat.Vector3,
                VertexElementUsage.Position, 0),
             new VertexElement(sizeof(float)*3, VertexElementFormat.Single,
                VertexElementUsage.PointSize, 0),
             new VertexElement(sizeof(float)*4, VertexElementFormat.Color,
                VertexElementUsage.Color, 0)
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
