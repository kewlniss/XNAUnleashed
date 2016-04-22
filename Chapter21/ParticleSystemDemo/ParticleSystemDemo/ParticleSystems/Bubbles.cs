using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XELibrary;

namespace ParticleSystemDemo
{
    public class Bubbles : XELibrary.ParticleSystem
    {
        public Bubbles(Game game, int capacity, Vector3 externalForce)
            : base(game)
        {
            settings.Capacity = capacity;
            settings.ExternalForce = externalForce;
        }

        public Bubbles(Game game, int capacity)
            : this(game, capacity, new Vector3(0, .05f, 0)) { }

        public Bubbles(Game game) : this(game, 5000) { }

        protected override ParticleSystemSettings InitializeSettings()
        {
            settings.EmitPerSecond = 1000;

            settings.EmitPosition = new Vector3(0, 0, -500);
            settings.EmitRange = new Vector3(50, 60, 50);

            settings.MinimumVelocity = new Vector3(-1, -1, -1);
            settings.MaximumVelocity = new Vector3(1, 20, 1);

            settings.MinimumAcceleration = new Vector3(-0.1f, -0.1f, -0.1f);
            settings.MaximumAcceleration = new Vector3(.1f, .1f, .1f);

            settings.MinimumLifetime = 1.0f;
            settings.MaximumLifetime = 25.0f;

            settings.MinimumSize = 5.0f;
            settings.MaximumSize = 15.0f;

            settings.Colors = new Color[] {
                Color.WhiteSmoke,
                Color.White,
                Color.NavajoWhite,
                Color.Khaki};

            settings.DisplayColorsInOrder = false;

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
