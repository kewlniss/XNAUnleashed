using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateDemo
{
    public sealed class LostGameState : BaseGameState, ILostGameState
    {
        SpriteFont font;

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
            OurGame.SpriteBatch.DrawString(font,
                "You Lost!!!", new Vector2(50, 50),
                Color.Firebrick, 0, Vector2.Zero, 3.0f,
                SpriteEffects.None, 0);

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
                OurGame.FadingState.Color = Color.Red;
                GameManager.PushState(OurGame.FadingState.Value);
            }
        }
    }
}
