using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XELibrary;

namespace ParticleSystemDemo
{
    public class PosionGas : XELibrary.ParticleSystem
    {
        public PosionGas(Game game, int capacity, Vector3 externalForce)
            : base(game)
        {
            settings.Capacity = capacity;
            settings.ExternalForce = externalForce;
        }

        public PosionGas(Game game, int capacity)
            : this(game, capacity, new Vector3(0, 0, 0)) { }

        public PosionGas(Game game) : this(game, 5000) { }

        protected override ParticleSystemSettings InitializeSettings()
        {
            settings.EmitPosition = new Vector3(0, -2500, -4000);
            settings.EmitPerSecond = 500;

            settings.EmitRadius = new Vector2(10, 10);

            settings.MinimumVelocity = new Vector3(-.1f, 0, -.1f);
            settings.MaximumVelocity = new Vector3(.1f, 0, .1f);

            settings.MinimumAcceleration = new Vector3(-.5f, .6f, -.5f);
            settings.MaximumAcceleration = new Vector3(.5f, .6f, .5f);

            settings.MinimumLifetime = 5.0f;
            settings.MaximumLifetime = 20.0f;

            settings.MinimumSize = 50.0f;
            settings.MaximumSize = 50.0f;

            settings.Colors = new Color[] {
                new Color(new Vector4(Color.Green.ToVector3(), .05f))
            };

            settings.DisplayColorsInOrder = false;

            settings.RotateAmount = 0.025f;

            settings.RunOnce = true;

            return (settings);
        }


        protected override void  LoadContent()
        {
            SetTexture(
                base.Game.Content.Load<Texture2D>(@"Textures\smoke"));

            base.LoadContent();
        }
    }
}
