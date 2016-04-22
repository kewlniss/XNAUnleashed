using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TunnelVision
{
    public sealed class WonGameState : BaseGameState, IWonGameState
    {
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
            Vector2 viewport = new Vector2(GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);
            Vector2 fontLength = OurGame.Font.MeasureString("You Won!!!");
            Vector2 pos = (viewport - fontLength * 3) / 2;

            OurGame.SpriteBatch.Begin();
            OurGame.SpriteBatch.DrawString(OurGame.Font,
                "You Won!!!", pos,
                Color.White, 0, Vector2.Zero, 3.0f,
                SpriteEffects.None, 0);
            OurGame.SpriteBatch.End();

            base.Draw(gameTime);
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
