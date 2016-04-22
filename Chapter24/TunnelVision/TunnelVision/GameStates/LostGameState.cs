using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TunnelVision
{
    public sealed class LostGameState : BaseGameState, ILostGameState
    {
        public LostGameState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(ILostGameState), this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 viewport = new Vector2(GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);
            Vector2 fontLength = OurGame.Font.MeasureString("You Lost!");
            Vector2 pos = (viewport - fontLength * 3) / 2;

            OurGame.SpriteBatch.Begin();
            OurGame.SpriteBatch.DrawString(OurGame.Font,
                "You Lost!!!", pos,
                Color.Firebrick, 0, Vector2.Zero, 3.0f,
                SpriteEffects.None, 0);
            OurGame.SpriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void StateChanged(object sender, EventArgs e)
        {
            if (GameManager.State == this.Value)
            {
                OurGame.FadingState.Color = Color.Red;
                //OurGame.Sound.Play("Death");
                GameManager.PushState(OurGame.FadingState.Value);
            }
        }
    }
}
