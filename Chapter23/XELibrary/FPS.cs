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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public sealed class FPS : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private float fps;
        private float updateInterval = 1.0f;
        private float timeSinceLastUpdate = 0.0f;
        private float framecount = 0;

        private bool inSpriteBatch = false;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private string fpsText = string.Empty;

        public FPS(Game game)
            : this(game, false, false, game.TargetElapsedTime) { }

        public FPS(Game game, bool synchWithVerticalRetrace, bool isFixedTimeStep)
            : this(game, synchWithVerticalRetrace, isFixedTimeStep,
                   game.TargetElapsedTime)
        { }

        public FPS(Game game, bool synchWithVerticalRetrace,
                   bool isFixedTimeStep, TimeSpan targetElapsedTime)
            : base(game)
        {
            GraphicsDeviceManager graphics =
                (GraphicsDeviceManager)Game.Services.GetService(
                typeof(IGraphicsDeviceManager));

            graphics.SynchronizeWithVerticalRetrace = synchWithVerticalRetrace;
            Game.IsFixedTimeStep = isFixedTimeStep;
            Game.TargetElapsedTime = targetElapsedTime;

            DrawOrder = 1000;
        }


        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public sealed override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        public void Load(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        protected sealed override void LoadContent()
        {
            font = Game.Content.Load<SpriteFont>(@"Fonts\arial");

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public sealed override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public sealed override void Draw(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            framecount++;
            timeSinceLastUpdate += elapsed;
            if (timeSinceLastUpdate > updateInterval)
            {
                fps = framecount / timeSinceLastUpdate;

                fpsText = "FPS: " + fps;

                framecount = 0;
                timeSinceLastUpdate -= updateInterval;
            }

            if (spriteBatch == null)
                throw new InvalidOperationException("Load must be called before Draw");

            if (!inSpriteBatch)
                spriteBatch.Begin(SpriteSortMode.Immediate);

            spriteBatch.DrawString(font, fpsText, Vector2.Zero, Color.Yellow);

            if (!inSpriteBatch)
                spriteBatch.End();

            base.Draw(gameTime);
        }

        public bool InSpriteBatch
        {
            get { return (inSpriteBatch); }
            set { inSpriteBatch = value; }
        }
    }
}