using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XELibrary;

namespace GameStateDemo
{
    public sealed class PausedState : BaseGameState, IPausedState
    {
        SpriteFont font;

        public PausedState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IPausedState), this);
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.WasPressed(0, Buttons.Start, Keys.Enter))
                GameManager.PopState(); //I am no longer paused ... 

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            OurGame.SpriteBatch.DrawString(font, "PAUSED", new Vector2(50, 20), Color.Yellow);
            base.Draw(gameTime);
        }

        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>(@"Fonts\Arial");
        }
    }
}
