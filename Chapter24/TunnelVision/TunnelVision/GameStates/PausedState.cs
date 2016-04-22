using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XELibrary;

namespace TunnelVision
{
    public sealed class PausedState : BaseGameState, IPausedState
    {
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
            Vector2 viewport = new Vector2(GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);
            Vector2 fontLength = OurGame.Font.MeasureString("PAUSED");
            Vector2 pos = (viewport - fontLength) / 2;

            OurGame.SpriteBatch.Begin();
            OurGame.SpriteBatch.DrawString(OurGame.Font, "PAUSED", pos, Color.Yellow);
            OurGame.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
