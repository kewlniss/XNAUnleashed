using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XELibrary;

namespace GameStateDemo
{
    public sealed class StartLevelState : BaseGameState, IStartLevelState
    {
        SpriteFont font;
        bool demoMode = true; //would obviously be read in from some setting ...
        bool displayedDemoDialog = false;
        public StartLevelState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IStartLevelState), this);
        }

        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>(@"Fonts\Arial");
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.WasPressed(0, Buttons.Start, Keys.Enter))
                GameManager.ChangeState(OurGame.PlayingState.Value); // change state to playing
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            OurGame.SpriteBatch.DrawString(font,
                "Starting Level ...", new Vector2(50, 50), Color.Black);
            base.Draw(gameTime);
        }

        protected override void StateChanged(object sender, EventArgs e)
        {
            base.StateChanged(sender, e);

            if (GameManager.State == this.Value)
            {
                if (demoMode && !displayedDemoDialog)
                {
                    //We could set properties on our YesNoDialog
                    //so it could have a custom message and custom
                    //Yes / No buttons ...
                    //YesNoDialogState.YesCaption = "Of course!";
                    GameManager.PushState(OurGame.YesNoDialogState.Value);
                    this.Visible = true;
                    displayedDemoDialog = true;
                }
            }
        }
    }
}
