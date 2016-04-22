using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XELibrary;

namespace GameStateDemo
{
    public sealed class StartMenuState : BaseGameState, IStartMenuState
    {
        private Texture2D texture;

        public StartMenuState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IStartMenuState), this);
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.WasPressed(0, Buttons.Back, Keys.Escape))
            {
                //go back to title / intro screen
                GameManager.ChangeState(OurGame.TitleIntroState.Value);
            }

            if (Input.WasPressed(0, Buttons.Start, Keys.Enter))
            {
                //got here from our playing state,just pop myself off the stack
                if (GameManager.ContainsState(OurGame.PlayingState.Value))
                    GameManager.PopState();
                else //starting game, queue first level
                    GameManager.ChangeState(OurGame.StartLevelState.Value); 
            }

            //options menu
            if (Input.WasPressed(0, Buttons.Y, Keys.O))
                GameManager.PushState(OurGame.OptionsMenuState.Value);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 pos = new Vector2(TitleSafeArea.Left, TitleSafeArea.Top);
            OurGame.SpriteBatch.Draw(texture, pos, Color.White);

            base.Draw(gameTime);
        }

        protected override void StateChanged(object sender, EventArgs e)
        {
            base.StateChanged(sender, e);

            if (GameManager.State != this.Value)
                Visible = true;
        }

        protected override void LoadContent()
        {
            texture = Content.Load<Texture2D>(@"Textures\startMenu");
        }
    }
}
