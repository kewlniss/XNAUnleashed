using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XELibrary;

namespace ParticleSystemDemo
{
    public class LaserScanner : LaserShield
    {
        public LaserScanner(Game game, int capacity, Vector3 externalForce)
            : base(game, capacity, externalForce) { }

        public LaserScanner(Game game, int capacity)
            : base(game, capacity) { }

        public LaserScanner(Game game) : base(game) { }

        protected override ParticleSystemSettings InitializeSettings()
        {
            base.InitializeSettings();

            settings.EmitPerSecond = (int)(settings.Capacity * 4);

            settings.Colors = new Color[] {
                new Color(new Vector4(Color.Red.ToVector3(), .025f))
            };

            return (settings);
        }
    }
}
