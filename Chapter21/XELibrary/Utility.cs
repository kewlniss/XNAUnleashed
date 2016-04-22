using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XELibrary
{
    public sealed class Utility
    {
        private static Random rand = new Random();

        public static int GetRandomInt(int min, int max)
        {
            return (rand.Next(min, max));
        }
        public static float GetRandomFloat(float min, float max)
        {
            return (((float)rand.NextDouble() * (max - min)) + min);
        }
        public static Vector2 GetRandomVector2(Vector2 min, Vector2 max)
        {
            return (new Vector2(
                GetRandomFloat(min.X, max.X),
                GetRandomFloat(min.Y, max.Y)));
        }
        public static Vector3 GetRandomVector3(Vector3 min, Vector3 max)
        {
            return (new Vector3(
                GetRandomFloat(min.X, max.X),
                GetRandomFloat(min.Y, max.Y),
                GetRandomFloat(min.Z, max.Z)));
        }

        public static Rectangle GetTitleSafeArea(GraphicsDevice graphicsDevice,
            float percent)
        {
            Rectangle retval = new Rectangle(graphicsDevice.Viewport.X,
                graphicsDevice.Viewport.Y,
                graphicsDevice.Viewport.Width,
                graphicsDevice.Viewport.Height);
#if XBOX360
            // Find Title Safe area of Xbox 360
            float border = (1 - percent) / 2;
            retval.X = (int)(border * retval.Width);
            retval.Y = (int)(border * retval.Height);
            retval.Width = (int)(percent * retval.Width);
            retval.Height = (int)(percent * retval.Height);
            return retval;            
#else
            return retval;
#endif
        }
    }
}
