using System;
using Microsoft.Xna.Framework;

namespace TunnelVision
{
    public abstract class SceneObject
    {
        public Matrix World;
        public BoundingSphere BoundingSphere;
    }

}
