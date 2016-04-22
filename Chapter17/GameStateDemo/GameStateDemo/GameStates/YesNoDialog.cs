using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XELibrary;

namespace GameStateDemo
{
    public sealed class YesNoDialogState : BaseGameState, IYesNoDialogState
    {
        SpriteFont font;

        public YesNoDialogState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(YesNoDialogState), this);
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.WasPressed(0, Buttons.A, Keys.Enter))
                GameManager.PopState(); //we are done ...

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            OurGame.SpriteBatch.DrawString(font,
                "Are you REALLY SURE you want to do THAT!?!?",
                new Vector2(50, 250), Color.Green);

            base.Draw(gameTime);
        }

        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>(@"Fonts\Arial");
        }
    }
}
