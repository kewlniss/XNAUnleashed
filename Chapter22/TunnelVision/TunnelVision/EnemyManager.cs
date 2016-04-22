using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TunnelVision
{
    public class EnemyManager : DrawableGameComponent
    {
        public const int MAX_ENEMIES = 10;
        private Texture2D[] enemyTextures;
        private Model enemy;
        private Effect effect;
        private Random rand = new Random();

        public Matrix View;
        public Matrix Projection;

        public List<Enemy> Enemies = new List<Enemy>(MAX_ENEMIES);

        public EnemyManager(Game game)
            : base(game) { }

        protected override void LoadContent()
        {
            enemyTextures = new Texture2D[3];
            enemyTextures[0] = Game.Content.Load<Texture2D>(
                @"Textures\wedge_p2_diff_v1");
            enemyTextures[1] = Game.Content.Load<Texture2D>(
                @"Textures\wedge_p2_diff_v2");
            enemyTextures[2] = Game.Content.Load<Texture2D>(
                @"Textures\wedge_p2_diff_v3");
            enemy = Game.Content.Load<Model>(@"Models\p2_wedge");
            effect = Game.Content.Load<Effect>(
                @"Effects\AmbientTexture");

            effect.Parameters["AmbientColor"].SetValue(.8f);

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            Enemies.Clear();
            Enemies = null;

            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            //GraphicsDevice.RenderState.AlphaTestEnable = false;
            //GraphicsDevice.RenderState.AlphaBlendEnable = false;
            //GraphicsDevice.RenderState.PointSpriteEnable = false;
            //GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            //GraphicsDevice.RenderState.DepthBufferEnable = true;

            effect.Parameters["View"].SetValue(View);
            effect.Parameters["Projection"].SetValue(Projection);

            for (int ei = 0; ei < Enemies.Count; ei++)
            {
                effect.Parameters["World"].SetValue(Enemies[ei].World);
                effect.Parameters["ColorMap"].SetValue(Enemies[ei].Texture);

                Matrix[] transforms = new Matrix[enemy.Bones.Count];
                enemy.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in enemy.Meshes)
                {
                    foreach (ModelMeshPart mp in mesh.MeshParts)
                    {
                        mp.Effect = effect;
                    }

                    mesh.Draw();
                }
            }

            base.Draw(gameTime);
        }

        public void AddEnemy(float moveSpeed)
        {
            Enemies.Add(new Enemy(enemyTextures[rand.Next(0, 3)], moveSpeed));
        }

    }

    public class Enemy : PhysicalObject
    {
        Random random = new Random(DateTime.Now.Millisecond);

        public Vector3 Target = Vector3.Zero;
        public Texture2D Texture;
        private float moveSpeed;
        private Vector3 Up, Forward;

        public Enemy(Texture2D texture, float moveSpeed)
        {
            Texture = texture;
            this.moveSpeed = moveSpeed;
            Scale = 0.01f;
            Radius = 5f;
            Position = XELibrary.Utility.GetRandomVector3(
                new Vector3(-300, -100, -100), new Vector3(300, 100, -100));

            Up = Vector3.Up;
            Forward = Vector3.Forward;
        }

        public override void Move(float elapsed)
        {
            Vector3 tv = Target - Position;
            tv.Normalize();

            Velocity = tv * moveSpeed;

            Forward = tv;

            Vector3 Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.Up));

            Up = Vector3.Normalize(Vector3.Cross(Right, Forward));

            Rotation = Matrix.Identity;
            Rotation.Forward = Forward;
            Rotation.Up = Up;
            Rotation.Right = Right;

            base.Move(elapsed);
        }
    }

}