using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using XELibrary;

namespace ParticleSystemDemo
{
    public class LaserShield : XELibrary.ParticleSystem
    {
        public LaserShield(Game game, int capacity, Vector3 externalForce)
            : base(game)
        {
            settings.Capacity = capacity;
            settings.ExternalForce = externalForce;
        }

        public LaserShield(Game game, int capacity)
            : this(game, capacity, new Vector3(0, 0, 0)) { }

        public LaserShield(Game game) : this(game, 2500) { }

        protected override ParticleSystemSettings InitializeSettings()
        {
            settings.EmitPosition = new Vector3(0, -1500, -4000);
            settings.EmitPerSecond = settings.Capacity;

            settings.EmitRadius = new Vector2(1200, 1200);

            settings.MinimumVelocity = new Vector3(0, 0, 0);
            settings.MaximumVelocity = new Vector3(0, 0, 0);

            settings.MinimumAcceleration = new Vector3(0, 100, 0);
            settings.MaximumAcceleration = new Vector3(0, 100, 0);

            settings.MinimumLifetime = 1.0f;
            settings.MaximumLifetime = 1.0f;

            settings.MinimumSize = 50.0f;
            settings.MaximumSize = 50.0f;

            settings.Colors = new Color[] {
        new Color(new Vector4(Color.SteelBlue.ToVector3(), .1f)),
        new Color(new Vector4(Color.Silver.ToVector3(), .1f))
    };
            settings.DisplayColorsInOrder = false;

            return (settings);
        }


        protected override void  LoadContent()
        {
            SetTexture(
                base.Game.Content.Load<Texture2D>(@"Textures\3dtubegray"));

            base.LoadContent();
        }
    }
}
