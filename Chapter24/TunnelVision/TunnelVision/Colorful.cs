using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XELibrary;

namespace TunnelVision
{
    public class Colorful : XELibrary.ParticleSystem
    {
        public Colorful(Game game, int capacity, Vector3 externalForce)
            : base(game)
        {
            settings.Capacity = capacity;
            settings.ExternalForce = externalForce;
        }

        public Colorful(Game game, int capacity)
            : this(game, capacity, new Vector3(0, .05f, 0)) { }

        public Colorful(Game game) : this(game, 250) { }

        protected override ParticleSystemSettings InitializeSettings()
        {
            settings.EmitPerSecond = 250;

            settings.EmitPosition = new Vector3(0, 0, -1000);
            settings.EmitRange = new Vector3(2, 2, 2);
            settings.EmitRadius = new Vector2(3, 3);

            settings.MinimumVelocity = new Vector3(-1, -1, -1);
            settings.MaximumVelocity = new Vector3(1, 20, 1);

            settings.MinimumAcceleration = new Vector3(-5, -10, -5);
            settings.MaximumAcceleration = new Vector3(5, 10, 5);

            settings.MinimumLifetime = 0.2f;
            settings.MaximumLifetime = 0.2f;

            settings.MinimumSize = 3.0f;
            settings.MaximumSize = 3.0f;

            settings.Colors = new Color[]
            {
                Color.White,
                Color.Yellow,
                Color.Orange,
                Color.Red,
                Color.Black
            };

            settings.DisplayColorsInOrder = true;

            settings.RunOnce = true;

            return (settings);
        }



        protected override void  LoadContent()
        {
            SetTexture(
                base.Game.Content.Load<Texture2D>(@"Textures\bubble"));

            base.LoadContent();
        }
    }
}
