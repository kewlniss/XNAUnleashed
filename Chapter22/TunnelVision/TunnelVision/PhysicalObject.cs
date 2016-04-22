using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TunnelVision
{
    public abstract class PhysicalObject : SceneObject
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Acceleration;
        public float Mass;
        public float Scale = 1.0f;
        public float Radius = 1.0f;
        public Color Color;
        public Matrix Rotation = Matrix.Identity;

        public virtual void Move(float elapsed)
        {
            //adjust velocity with our acceleration
            Velocity += Acceleration;

            //adjust position with our velocity
            Position += elapsed * Velocity;

            World = Matrix.CreateScale(Scale) * Rotation *
                Matrix.CreateTranslation(Position);

            BoundingSphere = new BoundingSphere(Position, Radius);
        }
    }
}