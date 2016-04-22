using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XELibrary
{
    public sealed class Utility
    {
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
