using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XELibrary
{
    public class Skybox : Microsoft.Xna.Framework.GameComponent
    {
        private Model skyboxModel;

        private Texture2D[] skyboxTextures;

        public Effect Effect;

        public Skybox(Game game) : base(game)
        {
        }
        
        public void Load(string skyboxName)
        {
            skyboxModel = Game.Content.Load<Model>(skyboxName);
            skyboxTextures = new Texture2D[6];
            int i = 0;
            foreach (ModelMesh mesh in skyboxModel.Meshes)
                foreach (BasicEffect currentEffect in mesh.Effects)
                    skyboxTextures[i++] = currentEffect.Texture;

            if (Effect != null)
            {
                foreach (ModelMesh mesh in skyboxModel.Meshes)
                    foreach (ModelMeshPart meshPart in mesh.MeshParts)
                        meshPart.Effect = Effect.Clone();
            }
        }

        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        public void Draw(Matrix view, Matrix projection, Matrix world)
        {

            if (skyboxModel == null)
                throw (new Exception("You must call Load before calling Draw"));

            foreach (ModelMesh mesh in skyboxModel.Meshes)
            {
                int i=0;
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.Projection = projection;
                    be.View = view;
                    be.World = world;
                    be.Texture = skyboxTextures[i++];
                    be.TextureEnabled = true;
                }
                mesh.Draw();
            }
        }
        
    }
}
