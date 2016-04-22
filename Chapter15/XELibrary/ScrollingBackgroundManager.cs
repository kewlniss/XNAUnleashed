using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace XELibrary
{
    public interface IScrollingBackgroundManager { }

    public class ScrollingBackgroundManager
        : Microsoft.Xna.Framework.GameComponent, IScrollingBackgroundManager
    {
        private Dictionary<string, ScrollingBackground> backgrounds =
            new Dictionary<string, ScrollingBackground>();
        private Dictionary<string, Texture2D> textures =
            new Dictionary<string, Texture2D>();

        private string contentPath;

        private int screenWidth;
        private int screenHeight;

        private float scrollRate;

        public ScrollingBackgroundManager(Game game, string contentPath)
            : base(game)
        {
            this.contentPath = contentPath;

            if (this.contentPath.LastIndexOf('\\') < this.contentPath.Length-1)
                this.contentPath += '\\';

            game.Services.AddService(
                typeof(IScrollingBackgroundManager), this);
        }

        public override void Initialize()
        {
            base.Initialize();

            GraphicsDeviceManager graphics =
                (GraphicsDeviceManager)Game.Services.GetService(
                typeof(IGraphicsDeviceManager));

            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;
        }

        public void AddBackground(string backgroundKey, string textureName,
            Vector2 position, float scrollRateRatio)
        {
            AddBackground(backgroundKey, textureName, position, null,
                scrollRateRatio, Color.White);
        }

        public void AddBackground(string backgroundKey, string textureName,
            Vector2 position, Rectangle? sourceRect, float scrollRateRatio,
            Color color)
        {
            ScrollingBackground background = new ScrollingBackground(
                textureName, position, sourceRect, scrollRateRatio, color);
            
            background.ScrollRate = scrollRate * scrollRateRatio;

            if (!textures.ContainsKey(textureName))
            {
                textures.Add(textureName, Game.Content.Load<Texture2D>(
                    contentPath + textureName));
            }

            if (backgrounds.ContainsKey(backgroundKey))
                backgrounds[backgroundKey] = background;
            else
                backgrounds.Add(backgroundKey, background);
        }

        public override void  Update(GameTime gameTime)
        {
            foreach (KeyValuePair<string, ScrollingBackground> background
                                                                in backgrounds)
            {
                ScrollingBackground sb = background.Value;

                sb.Position.X += (sb.ScrollRate *
                    (float)gameTime.ElapsedGameTime.TotalSeconds);
                sb.Position.X = sb.Position.X % textures[sb.TextureName].Width;
            }

            base.Update(gameTime);
        }

        ScrollingBackground sb;
        Texture2D texture;
        public void Draw(string backgroundKey,
            SpriteBatch batch)
        {
            sb = backgrounds[backgroundKey];
            texture = textures[sb.TextureName];

            //Draw the main texture
            batch.Draw(texture, sb.Position, sb.SourceRect,
                 sb.Color, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);

            //Determine if we need to scroll left or right
            Vector2 offset;
            if (sb.Positive)
                offset = sb.Position - (new Vector2(texture.Width, 0));
            else
                offset = new Vector2(texture.Width, 0) + sb.Position;
            //now draw the background again at the appropriate offset
            //NOTE: If our Width is larger than two times the size of our
            //texture then the code will need to be modified
            batch.Draw(texture, offset, sb.SourceRect,
                 sb.Color, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }

        public float ScrollRate
        {
            get { return(scrollRate); }
        }

        public void SetScrollRate(float scrollRate)
        {
            this.scrollRate = scrollRate;

           foreach (ScrollingBackground sb in backgrounds.Values)
               sb.ScrollRate = scrollRate * sb.ScrollRateRatio;
        }
    }

    public class ScrollingBackground
    {
        public Rectangle? SourceRect;
        public string TextureName;
        public Vector2 Position;
        public float ScrollRateRatio;
        public float ScrollRate;
        public Color Color;

        public bool Positive
        {
            get { return (ScrollRate > 0); }
        }

        public ScrollingBackground(string textureName, Vector2 position,
            Rectangle? sourceRect, float scrollRateRatio, Color color)
        {
            TextureName = textureName;
            Position = position;
            SourceRect = sourceRect;
            ScrollRateRatio = scrollRateRatio;
            Color = color;
        }
    }
}