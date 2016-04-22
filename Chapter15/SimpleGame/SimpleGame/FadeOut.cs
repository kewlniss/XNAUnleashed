using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace SimpleGame
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class FadeOut : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Texture2D fadeTexture;
        private float fadeAmount;
        private double fadeStartTime;
        private SimpleGame simpleGame;

        public Color Color;
        private SpriteBatch spriteBatch;

        public FadeOut(Game game)
            : base(game)
        {
            simpleGame = (SimpleGame)game;

            this.Enabled = false;
            this.Visible = false;

            DrawOrder = 999;
        }


        public void Load(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (fadeStartTime == 0)
            {
                fadeStartTime = gameTime.TotalGameTime.TotalMilliseconds;
                Visible = true;
            }

            fadeAmount += (.25f * (float)gameTime.ElapsedGameTime.TotalSeconds);

            if (gameTime.TotalGameTime.TotalMilliseconds > fadeStartTime + 4000)
            {
                fadeAmount = 0;
                fadeStartTime = 0;
                Visible = Enabled = false;

                simpleGame.ActivateStartMenu();
            }

            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
#if ZUNE
            fadeTexture = CreateFadeTexture(
                GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Width);
#else
            fadeTexture = CreateFadeTexture(
                GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
#endif
        }

        public override void Draw(GameTime gameTime)
        {           
            Vector4 color = Color.ToVector4();
            color.W = fadeAmount; //set transparancy
            spriteBatch.Draw(fadeTexture, Vector2.Zero, new Color(color));

            base.Draw(gameTime);
        }

        private Texture2D CreateFadeTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, width, height, true, SurfaceFormat.Color);

            int pixelCount = width * height;
            Color[] pixelData = new Color[pixelCount];
            Random rnd = new Random();

            for (int i = 0; i < pixelCount; i++)
            {
                pixelData[i] = Color.White;
            }

            texture.SetData(pixelData);

            return (texture);
        }
    }
}