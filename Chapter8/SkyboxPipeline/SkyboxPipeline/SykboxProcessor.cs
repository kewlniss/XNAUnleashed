using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using TInput = Microsoft.Xna.Framework.Content.Pipeline.Graphics.Texture2DContent;
using TOutput = SkyboxPipeline.SkyboxContent;

using System.ComponentModel;

namespace SkyboxPipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentProcessor(DisplayName = "SkyboxProcessor")]
    public class SykboxProcessor : ContentProcessor<TInput, TOutput>
    {
        private int width = 1024;
        private int height = 512;
        private int cellSize = 256;

        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            MeshBuilder builder = MeshBuilder.StartMesh("XESkybox");

            CreatePositions(ref builder);

            AddVerticesInformation(ref builder);

            // Finish making the mesh
            MeshContent skyboxMesh = builder.FinishMesh();

            // Create the output object.
            SkyboxContent skybox = new SkyboxContent();

            //Compile the mesh we just built through the default ModelProcessor
            skybox.Model = context.Convert<MeshContent, ModelContent>(
                skyboxMesh, "ModelProcessor");

            skybox.Texture = input;

            return skybox;
        }

        //MeshBuilder isn't implemented in MonoGame it seems
        private void CreatePositions(ref MeshBuilder builder)
        {
            Vector3 position;

            //————near / back plane (behind the camera)
            //top left
            position = new Vector3(-1, 1, 1);
            builder.CreatePosition(position); //0

            //bottom right
            position = new Vector3(1, -1, 1);
            builder.CreatePosition(position); //1

            //bottom left
            position = new Vector3(-1, -1, 1);
            builder.CreatePosition(position); //2

            //top right
            position = new Vector3(1, 1, 1);
            builder.CreatePosition(position); //3

            //————far / front plane (in front of camera)
            //top left
            position = new Vector3(-1, 1, -1); //4
            builder.CreatePosition(position);

            //bottom right
            position = new Vector3(1, -1, -1); //5
            builder.CreatePosition(position);

            //bottom left
            position = new Vector3(-1, -1, -1); //6
            builder.CreatePosition(position);

            //top right
            position = new Vector3(1, 1, -1); //7
            builder.CreatePosition(position);
        }

        private Vector2 UV(int u, int v, Vector2 cellIndex)
        {
            return (new Vector2((cellSize * (cellIndex.X + u) / width),
                (cellSize * (cellIndex.Y + v) / height)));
        }

        private void AddVerticesInformation(ref MeshBuilder builder)
        {
            //texture locations:
            //F,R,B,L
            //U,D

            //Front
            Vector2 fi = new Vector2(0, 0); //cell 0, row 0

            //Right
            Vector2 ri = new Vector2(1, 0); //cell 1, row 0

            //Back
            Vector2 bi = new Vector2(2, 0); //cell 2, row 0

            //Left
            Vector2 li = new Vector2(3, 0); //cell 3, row 0

            //Upward (Top)
            Vector2 ui = new Vector2(0, 1); //cell 0, row 1

            //Downward (Bottom)
            Vector2 di = new Vector2(1, 1); //cell 1, row 1

            int texCoordChannel = builder.CreateVertexChannel<Vector2>
                (VertexChannelNames.TextureCoordinate(0));

            //————front plane first column, first row

            //bottom triangle of front plane
            builder.SetVertexChannelData(texCoordChannel, UV(0, 0, fi));
            builder.AddTriangleVertex(4); //-1,1,-1
            builder.SetVertexChannelData(texCoordChannel, UV(1, 1, fi));
            builder.AddTriangleVertex(5); //1,-1,-1
            builder.SetVertexChannelData(texCoordChannel, UV(0, 1, fi));
            builder.AddTriangleVertex(6); //-1,-1,-1

            //top triangle of front plane
            builder.SetVertexChannelData(texCoordChannel, UV(0, 0, fi));
            builder.AddTriangleVertex(4); //-1,1,-1
            builder.SetVertexChannelData(texCoordChannel, UV(1, 0, fi));
            builder.AddTriangleVertex(7); //1,1,-1
            builder.SetVertexChannelData(texCoordChannel, UV(1, 1, fi));
            builder.AddTriangleVertex(5); //1,-1,-1

            //————right plane
            builder.SetVertexChannelData(texCoordChannel, UV(1, 0, ri));
            builder.AddTriangleVertex(3);
            builder.SetVertexChannelData(texCoordChannel, UV(1, 1, ri));
            builder.AddTriangleVertex(1);
            builder.SetVertexChannelData(texCoordChannel, UV(0, 1, ri));
            builder.AddTriangleVertex(5);

            builder.SetVertexChannelData(texCoordChannel, UV(1, 0, ri));
            builder.AddTriangleVertex(3);
            builder.SetVertexChannelData(texCoordChannel, UV(0, 1, ri));
            builder.AddTriangleVertex(5);
            builder.SetVertexChannelData(texCoordChannel, UV(0, 0, ri));
            builder.AddTriangleVertex(7);

            //————back pane //3rd column, first row
            //bottom triangle of back plane
            builder.SetVertexChannelData(texCoordChannel, UV(1, 1, bi)); //1,1
            builder.AddTriangleVertex(2); //-1,-1,1
            builder.SetVertexChannelData(texCoordChannel, UV(0, 1, bi)); //0,1
            builder.AddTriangleVertex(1); //1,-1,1
            builder.SetVertexChannelData(texCoordChannel, UV(1, 0, bi)); //1,0
            builder.AddTriangleVertex(0); //-1,1,1

            //top triangle of back plane
            builder.SetVertexChannelData(texCoordChannel, UV(0, 1, bi)); //0,1
            builder.AddTriangleVertex(1); //1,-1,1
            builder.SetVertexChannelData(texCoordChannel, UV(0, 0, bi)); //0,0
            builder.AddTriangleVertex(3); //1,1,1
            builder.SetVertexChannelData(texCoordChannel, UV(1, 0, bi)); //1,0
            builder.AddTriangleVertex(0); //-1,1,1

            //————left plane
            builder.SetVertexChannelData(texCoordChannel, UV(1, 1, li));
            builder.AddTriangleVertex(6);
            builder.SetVertexChannelData(texCoordChannel, UV(0, 1, li));
            builder.AddTriangleVertex(2);
            builder.SetVertexChannelData(texCoordChannel, UV(0, 0, li));
            builder.AddTriangleVertex(0);

            builder.SetVertexChannelData(texCoordChannel, UV(1, 0, li));
            builder.AddTriangleVertex(4);
            builder.SetVertexChannelData(texCoordChannel, UV(1, 1, li));
            builder.AddTriangleVertex(6);
            builder.SetVertexChannelData(texCoordChannel, UV(0, 0, li));
            builder.AddTriangleVertex(0);

            //————upward (top) plane
            builder.SetVertexChannelData(texCoordChannel, UV(1, 0, ui));
            builder.AddTriangleVertex(3);
            builder.SetVertexChannelData(texCoordChannel, UV(0, 1, ui));
            builder.AddTriangleVertex(4);
            builder.SetVertexChannelData(texCoordChannel, UV(0, 0, ui));
            builder.AddTriangleVertex(0);

            builder.SetVertexChannelData(texCoordChannel, UV(1, 0, ui));
            builder.AddTriangleVertex(3);
            builder.SetVertexChannelData(texCoordChannel, UV(1, 1, ui));
            builder.AddTriangleVertex(7);
            builder.SetVertexChannelData(texCoordChannel, UV(0, 1, ui));
            builder.AddTriangleVertex(4);

            //————downward (bottom) plane
            builder.SetVertexChannelData(texCoordChannel, UV(1, 0, di));
            builder.AddTriangleVertex(2);
            builder.SetVertexChannelData(texCoordChannel, UV(1, 1, di));
            builder.AddTriangleVertex(6);
            builder.SetVertexChannelData(texCoordChannel, UV(0, 0, di));
            builder.AddTriangleVertex(1);

            builder.SetVertexChannelData(texCoordChannel, UV(1, 1, di));
            builder.AddTriangleVertex(6);
            builder.SetVertexChannelData(texCoordChannel, UV(0, 1, di));
            builder.AddTriangleVertex(5);
            builder.SetVertexChannelData(texCoordChannel, UV(0, 0, di));
            builder.AddTriangleVertex(1);
        }
    }
}