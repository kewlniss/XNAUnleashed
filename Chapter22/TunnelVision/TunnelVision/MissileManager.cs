using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TunnelVision
{

    public class MissileManager : DrawableGameComponent
    {
        public const int MISSILE_LIFE = 5; //5 seconds
        private Model missile;
        private Effect effect;

        public Matrix View;
        public Matrix Projection;

        private Missile[] missiles;
        private Texture2D missileTexture;

        private int lastMissileIndex;
        private float timer = 0;

        public MissileManager(Game game)
            : base(game) { }

        public void Load(int capacity)
        {
            missiles = new Missile[capacity];
            lastMissileIndex = 0;

            for (int i = 0; i < missiles.Length; i++)
                missiles[i] = new Missile();
        }

        protected override void LoadContent()
        {
            missileTexture = Game.Content.Load<Texture2D>(
                @"Textures\FireGrade");
            missile = Game.Content.Load<Model>(@"Models\sphere");
            effect = Game.Content.Load<Effect>(
                @"Effects\VertexDisplacement");
            effect.Parameters["ColorMap"].SetValue(missileTexture);
            effect.Parameters["AmbientColor"].SetValue(0.8f);

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            missiles = null;

            base.UnloadContent();
        }

        public bool AddMissile(Vector3 position, Vector3 direction, DateTime startTime)
        {
            int index = lastMissileIndex;
            for (int i = 0; i < missiles.Length; i++)
            {
                if (!missiles[index].IsActive)
                    break;
                else
                {
                    index++;
                    if (index >= missiles.Length)
                        index = 0;
                }

                if (index == lastMissileIndex)
                    return (false);
            }

            //at this point index is the one we want ...
            InitializeMissile(index, position, direction, startTime);

            missiles[index].IsActive = true;

            lastMissileIndex = index;

            return (true);
        }

        private void InitializeMissile(int index, Vector3 position, Vector3 direction,
            DateTime startTime)
        {
            missiles[index] = new Missile();
            missiles[index].Position = position;
            missiles[index].Acceleration = direction * 10f;
            missiles[index].Velocity = Vector3.Zero;
            missiles[index].StartTime = startTime;
        }

        public bool CheckCollision(BoundingSphere check)
        {
            for (int i = 0; i < missiles.Length; i++)
            {
                if ((missiles[i].IsActive) &&
                    (missiles[i].BoundingSphere.Intersects(check)))
                {
                    RemoveMissile(i);
                    return (true);
                }
            }

            return (false);
        }

        private void RemoveMissile(int index)
        {
            missiles[index].IsActive = false;
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            timer += 0.655f;

            for (int mi = 0; mi < missiles.Length; mi++)
            {
                if (!missiles[mi].IsActive)
                    continue;

                if ((DateTime.Now - missiles[mi].StartTime) >
                        TimeSpan.FromSeconds(MissileManager.MISSILE_LIFE))
                    RemoveMissile(mi);
                else
                    missiles[mi].Move(elapsed);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.RenderState.DepthBufferEnable = true;
            //GraphicsDevice.RenderState.AlphaBlendEnable = true;

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            effect.Parameters["Timer"].SetValue(timer);

            effect.Parameters["View"].SetValue(View);
            effect.Parameters["Projection"].SetValue(Projection);

            for (int mi = 0; mi < missiles.Length; mi++)
            {
                if (!missiles[mi].IsActive)
                    continue;

                effect.Parameters["World"].SetValue(missiles[mi].World);

                Matrix[] transforms = new Matrix[missile.Bones.Count];
                missile.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in missile.Meshes)
                {
                    for (int i = 0; i < mesh.MeshParts.Count; i++)
                    {
                        // Set this MeshParts effect to our RedWire effect
                        mesh.MeshParts[i].Effect = effect;
                    }

                    mesh.Draw();

                    missiles[mi].BoundingSphere = mesh.BoundingSphere;
                    missiles[mi].BoundingSphere.Center += missiles[mi].World.Translation;
                }
            }

            base.Draw(gameTime);
        }


    }

    class Missile : PhysicalObject
    {
        public DateTime StartTime;
        public bool IsActive;
    }
}
