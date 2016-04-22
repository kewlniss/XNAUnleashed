using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XELibrary
{
    public class ParticleSystemSettings
    {
        public Texture2D Texture;

        public float RotateAmount;

        public bool RunOnce = false;
        public int Capacity;
        public int EmitPerSecond;

        public Vector3 ExternalForce;

        public Vector3 EmitPosition;
        public Vector2 EmitRadius = Vector2.Zero;
        public Vector3 EmitRange;

        public Vector3 MinimumVelocity;
        public Vector3 MaximumVelocity;

        public Vector3 MinimumAcceleration;
        public Vector3 MaximumAcceleration;

        public float MinimumLifetime;
        public float MaximumLifetime;

        public float MinimumSize;
        public float MaximumSize;

        public Color[] Colors;
        public bool DisplayColorsInOrder;
    }

}
