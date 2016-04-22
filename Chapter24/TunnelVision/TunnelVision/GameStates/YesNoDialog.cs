using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XELibrary;

namespace TunnelVision
{
    public sealed class YesNoDialogState : BaseGameState, IYesNoDialogState
    {
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
            Vector2 viewport = new Vector2(GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);
            Vector2 fontLength =
                OurGame.Font.MeasureString("Are you REALLY SURE you want to do THAT!?!?");
            Vector2 pos = (viewport - fontLength) / 2;

            OurGame.SpriteBatch.Begin();
            OurGame.SpriteBatch.DrawString(OurGame.Font,
                "Are you REALLY SURE you want to do THAT!?!?",
                pos, Color.Green);
            OurGame.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
