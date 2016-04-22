using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using TRead = XELibrary.Skybox;

namespace XELibrary
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content
    /// Pipeline to read the specified data type from binary .xnb format.
    /// 
    /// Unlike the other Content Pipeline support classes, this should
    /// be a part of your main game project, and not the Content Pipeline
    /// Extension Library project.
    /// </summary>
    public class SkyboxReader : ContentTypeReader<TRead>
    {
        protected override TRead Read(ContentReader input, TRead existingInstance)
        {
            return new Skybox(input);
        }
    }

    public class Skybox
    {
        private Model skyboxModel;
        private Texture2D skyboxTexture;

        internal Skybox(ContentReader input)
        {
            skyboxModel = input.ReadObject<Model>();
            skyboxTexture = input.ReadObject<Texture2D>();
        }

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            foreach (ModelMesh mesh in skyboxModel.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.Projection = projection;
                    be.View = view;
                    be.World = world;
                    be.Texture = skyboxTexture;
                    be.TextureEnabled = true;
                }
                mesh.Draw(); // SaveStateMode.SaveState);
            }
        }
    }
}
