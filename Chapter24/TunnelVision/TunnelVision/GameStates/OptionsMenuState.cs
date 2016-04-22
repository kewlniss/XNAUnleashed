using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XELibrary;

namespace TunnelVision
{
    public sealed class OptionsMenuState : BaseGameState, IOptionsMenuState
    {
        private Texture2D texture;
        private GamePadState currentGamePadState;
        private GamePadState previousGamePadState;
        private Texture2D select;
        private Texture2D check;
        private int selected;


        public OptionsMenuState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IOptionsMenuState), this);
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.WasPressed(0, Buttons.Back, Keys.Escape))
                GameManager.PopState();

            if (Input.KeyboardState.WasKeyPressed(Keys.Up) ||
               (currentGamePadState.IsButtonDown(Buttons.DPadUp) &&
                previousGamePadState.IsButtonUp(Buttons.DPadUp)) ||
               (currentGamePadState.ThumbSticks.Left.Y > 0 &&
                previousGamePadState.ThumbSticks.Left.Y <= 0))
            {
                selected--;
            }
            if (Input.KeyboardState.WasKeyPressed(Keys.Down) ||
               (currentGamePadState.IsButtonDown(Buttons.DPadDown) &&
                previousGamePadState.IsButtonUp(Buttons.DPadDown)) ||
               (currentGamePadState.ThumbSticks.Left.Y < 0 &&
                previousGamePadState.ThumbSticks.Left.Y >= 0))
            {
                selected++;
            }
            if (selected < 0)
                selected = 1;
            if (selected == 2)
                selected = 0;

            if ((Input.WasPressed(0, Buttons.Start, Keys.Enter)) ||
                (Input.WasPressed(0, Buttons.A, Keys.Space)))
            {
                switch (selected)
                {
                    case 0: //Display Crosshairs
                        {
                            OurGame.DisplayCrosshair = !OurGame.DisplayCrosshair;
                            break;
                        }
                    case 1: //Display Radar
                        {
                            OurGame.DisplayRadar = !OurGame.DisplayRadar;
                            break;
                        }
                }
            }

            previousGamePadState = currentGamePadState;
            currentGamePadState = Input.GamePads[0];

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            Vector2 pos = new Vector2((GraphicsDevice.Viewport.Width - texture.Width) / 2,
                (GraphicsDevice.Viewport.Height - texture.Height) / 2);
            Vector2 crosshairCheckPos = new Vector2(pos.X + 142, pos.Y + 44);
            Vector2 radarCheckPos = new Vector2(pos.X + 142, pos.Y + 136);
            Vector2 crosshairSelectPos = new Vector2(pos.X + 145, pos.Y + 81);
            Vector2 radarSelectPos = new Vector2(pos.X + 145, pos.Y + 173);

            OurGame.SpriteBatch.Begin();
            OurGame.SpriteBatch.Draw(texture, pos, Color.White);

            if (OurGame.DisplayCrosshair)
                OurGame.SpriteBatch.Draw(check, crosshairCheckPos, Color.White);

            if (OurGame.DisplayRadar)
                OurGame.SpriteBatch.Draw(check, radarCheckPos, Color.White);

            switch (selected)
            {
                case 0: //Display Crosshairs
                    {
                        OurGame.SpriteBatch.Draw(select, crosshairSelectPos, Color.White);
                        break;
                    }
                case 1: //Display Radar
                    {
                        OurGame.SpriteBatch.Draw(select, radarSelectPos, Color.White);
                        break;
                    }
            } 

            OurGame.SpriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void LoadContent()
        {
            texture = Content.Load<Texture2D>(
                @"Textures\optionsMenu");
            check = Content.Load<Texture2D>(@"Textures\x");
            select = Content.Load<Texture2D>(@"Textures\select");

        }
    }
}
