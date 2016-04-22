using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XELibrary;

namespace ParticleSystemDemo
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

        public Colorful(Game game) : this(game, 2500) { }

        protected override ParticleSystemSettings InitializeSettings()
        {
            settings.EmitPerSecond = 2500;

            settings.EmitPosition = new Vector3(0, 0, -1000);
            settings.EmitRange = new Vector3(50, 160, 50);
            settings.EmitRadius = new Vector2(100, 100);

            settings.MinimumVelocity = new Vector3(-1, -1, -1);
            settings.MaximumVelocity = new Vector3(1, 20, 1);

            settings.MinimumAcceleration = new Vector3(-5, -10, -5);
            settings.MaximumAcceleration = new Vector3(5, 10, 5);

            settings.MinimumLifetime = 1.0f;
            settings.MaximumLifetime = 1.0f;

            settings.MinimumSize = 15.0f;
            settings.MaximumSize = 15.0f;

            settings.Colors = new Color[] {
                Color.Green,
                Color.Yellow,
                Color.Red,
                Color.Blue,
                Color.Purple};

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
