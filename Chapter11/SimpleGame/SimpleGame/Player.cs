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

    public class Player : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public string CurrentAnimation = "hero";
        public Vector2 Position;

        private float kickTime = 0.0f;
        private bool kicking = false;

        private InputHandler input;
        private CelAnimationManager cam;
        private SpriteBatch spriteBatch;

        public bool Attacking
        {
            // could OR ( || ) a bunch of other actions if we had them
            get { return (kicking); }
        }

        public Player(Game game)
            : base(game)
        {
            input = (InputHandler)game.Services.GetService(
                typeof(IInputHandler));
            cam = (CelAnimationManager)game.Services.GetService(
                typeof(ICelAnimationManager));
            Visible = Enabled = false;
        }

        public void Load(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        public void SetPause(bool paused)
        {
            cam.ToggleAnimation(CurrentAnimation, paused);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (WasPressed(0, Buttons.A, Keys.Space))
            {
                kickTime = 0;
                kicking = true;
            }

            if (kicking)
            {
                CurrentAnimation = "hero-kick";
                kickTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (kickTime > 0.5f)
                    kicking = false;
            }
            else
                CurrentAnimation = "hero";

            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            cam.AddAnimation("hero", "hero", new CelCount(4, 4), 16);
            cam.AddAnimation("hero-kick", "hero-kick", new CelCount(4, 1), 8);
        }

        public override void Draw(GameTime gameTime)
        {
            cam.Draw(gameTime, CurrentAnimation, spriteBatch, Position);

            base.Draw(gameTime);
        }

        private bool WasPressed(int playerIndex,
            Buttons button, Keys keys)
        {
            if (input.ButtonHandler.WasButtonPressed(playerIndex, button) ||
                    input.KeyboardState.WasKeyPressed(keys))
                return (true);
            else
                return (false);
        }
    }
}