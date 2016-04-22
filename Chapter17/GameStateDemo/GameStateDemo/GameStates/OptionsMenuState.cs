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
    public sealed class OptionsMenuState : BaseGameState, IOptionsMenuState
    {
        Texture2D texture;

        public OptionsMenuState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IOptionsMenuState), this);
        }

        public override void Update(GameTime gameTime)
        {
            if ((Input.WasPressed(0, Buttons.Back, Keys.Escape)
            || (Input.WasPressed(0, Buttons.Start, Keys.Enter))))
                GameManager.PopState();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 pos = new Vector2(TitleSafeArea.Left + 50,
                TitleSafeArea.Top + 50);
            OurGame.SpriteBatch.Draw(texture, pos, Color.White);

            base.Draw(gameTime);
        }

        protected override void LoadContent()
        {
            texture = Content.Load<Texture2D>(
                @"Textures\optionsMenu");
        }
    }
}
