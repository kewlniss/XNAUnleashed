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
    public class ProgressBar : Microsoft.Xna.Framework.GameComponent
    {
        private Texture2D progressBar;
        private readonly Vector2 initializationVector = new Vector2(-99, -99);
        private Vector2 currentPosition;
        private Vector2 originalPosition;
        private Vector2 position;
        //background area of our texture (256 - 63 = 193)
        private Rectangle progressBarBackground = new Rectangle(63, 0, 193, 32);
        //foreground of our texture
        private Rectangle progressBarForeground = new Rectangle(0, 0, 63, 20);
        // where we want our foreground to show up on our background
        private Vector2 progressBarOffset = new Vector2(7, 6);
        public float MoveRate = 90.0f;

        public ProgressBar(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            Enabled = false;
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if (Enabled)
                currentPosition = originalPosition = initializationVector;

            base.OnEnabledChanged(sender, args);
        }

        public void Load(Vector2 position)
        {
            progressBar = Game.Content.Load<Texture2D>(@"Textures\progressbar");
            this.position = position;
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
            if (currentPosition == initializationVector) //first time in
                currentPosition = originalPosition = position;
            else
                currentPosition += new Vector2(MoveRate *
                    (float)gameTime.ElapsedGameTime.TotalSeconds, 0);

            //have we reached the end (or the beginning) of our area?
            //If so reverse direction
            if (currentPosition.X > originalPosition.X +
                    (progressBarBackground.Width - progressBarForeground.Width - 15)
                || currentPosition.X < position.X)
            {
                MoveRate = -MoveRate;
            }

            base.Update(gameTime);
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Color color)
        {
            if (!Enabled)
                return;

            if (progressBar == null)
                throw (new Exception("You must call Load before calling Draw"));

            spriteBatch.Draw(progressBar, originalPosition, progressBarBackground,
                Color.White);
            spriteBatch.Draw(progressBar, currentPosition + progressBarOffset,
                progressBarForeground, color);
        }
    }
}