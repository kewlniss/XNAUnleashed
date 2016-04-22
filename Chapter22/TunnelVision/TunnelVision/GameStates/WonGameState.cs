using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TunnelVision
{
    public sealed class WonGameState : BaseGameState, IWonGameState
    {
        SpriteFont font;

        public WonGameState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IWonGameState), this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            OurGame.SpriteBatch.Begin();
            OurGame.SpriteBatch.DrawString(font,
                "You Won!!!", new Vector2(50, 50),
                Color.White, 0, Vector2.Zero, 3.0f,
                SpriteEffects.None, 0);
            OurGame.SpriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>(@"Fonts\Arial");
        }

        protected override void StateChanged(object sender, EventArgs e)
        {
            if (GameManager.State == this.Value)
            {
                OurGame.FadingState.Color = Color.Black;
                GameManager.PushState(OurGame.FadingState.Value);
            }
        }
    }
}
