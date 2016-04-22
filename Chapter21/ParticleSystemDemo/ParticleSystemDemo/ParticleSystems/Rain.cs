using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XELibrary;

namespace ParticleSystemDemo
{
    public class Rain : XELibrary.ParticleSystem
    {
        public Rain(Game game, int capacity, Vector3 externalForce)
            : base(game)
        {
            settings.Capacity = capacity;
            settings.ExternalForce = externalForce;
        }

        public Rain(Game game, int capacity)
            : this(game, capacity, new Vector3(0, 0, 0)) { }

        public Rain(Game game) : this(game, 5000) { }

        protected override ParticleSystemSettings InitializeSettings()
        {
            settings.EmitPerSecond = 1100;

            settings.EmitPosition = new Vector3(0, 4000, 0);
            settings.EmitRange = new Vector3(4000, 0, 4000);

            settings.MinimumVelocity = new Vector3(0, -10, 0);
            settings.MaximumVelocity = new Vector3(0, -50, 0);

            settings.MinimumAcceleration = new Vector3(0, -10, 0);
            settings.MaximumAcceleration = new Vector3(0, -10, 0);

            settings.MinimumLifetime = 5.0f;
            settings.MaximumLifetime = 5.0f;

            settings.MinimumSize = 5.0f;
            settings.MaximumSize = 15.0f;

            settings.Colors = new Color[] {
                Color.CornflowerBlue,
                Color.LightBlue
            };

            settings.DisplayColorsInOrder = false;

            return (settings);
        }

        protected override void  LoadContent()
        {
            SetTexture(
                base.Game.Content.Load<Texture2D>(@"Textures\raindrop"));

            base.LoadContent();
        }
    }
}
