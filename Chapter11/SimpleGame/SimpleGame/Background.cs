using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

using XELibrary;

namespace SimpleGame
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Background : Microsoft.Xna.Framework.DrawableGameComponent
    {

        private ScrollingBackgroundManager sbm;
        private SpriteBatch spriteBatch;


        public Background(Game game, string contentPath)
            : base(game)
        {
            sbm = new ScrollingBackgroundManager(game, contentPath);
            game.Components.Add(sbm);
        }

        public void Load(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        protected override void LoadContent()
        {
            int viewportWidth = GraphicsDevice.Viewport.Width;
            int viewportHeight = GraphicsDevice.Viewport.Height;
            int textureWidth = 1024;
            int cloudHeight = 512;
            int cityHeight = 256;
            int foregroundHeight = 128;
            int streetHeight = 128;
            int cloudY = 0;
            int streetY = cloudHeight;
            int foregroundY = streetY + streetHeight;
            int cityY = foregroundY + foregroundHeight;

            sbm.SetScrollRate(-75.0f); //75 pixels a second

            sbm.AddBackground("clouds1", "background",
                Vector2.Zero,
                new Rectangle(0, cloudY, textureWidth, cloudHeight),
                0.4f, Color.White);

            sbm.AddBackground("clouds2", "background",
                new Vector2(128, 0),
                new Rectangle(0, cloudY, textureWidth, cloudHeight),
                0.6f, new Color(255, 255, 255, 127));

            sbm.AddBackground("street", "background",
                new Vector2(0, viewportHeight - streetHeight),
                new Rectangle(0, streetY, textureWidth, streetHeight),
                1.0f, Color.White);

            sbm.AddBackground("foreground", "background",
                new Vector2(0, viewportHeight - foregroundHeight),
                new Rectangle(0, foregroundY, textureWidth, foregroundHeight),
                1.5f, Color.White);

            sbm.AddBackground("city", "background",
                new Vector2(0, viewportHeight - cityHeight - streetHeight + 64),
                new Rectangle(0, cityY, textureWidth, cityHeight),
                0.8f, Color.White);

        }


        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        BlendState cloudBlendState;

        public override void Draw(GameTime gameTime)
        {
            sbm.Draw("clouds1", spriteBatch);
            sbm.Draw("clouds2", spriteBatch);

            if (cloudBlendState == null)
            {
                cloudBlendState = new BlendState();
                cloudBlendState.ColorSourceBlend = Blend.SourceAlpha;
                cloudBlendState.ColorDestinationBlend = Blend.InverseSourceAlpha;
            }

            GraphicsDevice.BlendState = cloudBlendState;

            sbm.Draw("city", spriteBatch);
            sbm.Draw("street", spriteBatch);

            base.Draw(gameTime);
        }

    }
}