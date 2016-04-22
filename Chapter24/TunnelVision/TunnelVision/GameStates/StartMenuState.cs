using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XELibrary;

namespace TunnelVision
{
    public sealed class StartMenuState : BaseGameState, IStartMenuState
    {
        private Texture2D texture;
        private SpriteFont font;
        private GamePadState currentGamePadState;
        private GamePadState previousGamePadState;
        private int selected;

        private string[] entries = 
            {
                "Play",
                "Options",
                "High Scores",
                "Exit Game"
            };


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

            if (Input.KeyboardState.WasKeyPressed(Keys.Up) ||
               (currentGamePadState.DPad.Up == ButtonState.Pressed &&
                previousGamePadState.DPad.Up == ButtonState.Released) ||
               (currentGamePadState.ThumbSticks.Left.Y > 0 &&
                previousGamePadState.ThumbSticks.Left.Y <= 0))
            {
                selected--;
            }
            if (Input.KeyboardState.WasKeyPressed(Keys.Down) ||
               (currentGamePadState.DPad.Down == ButtonState.Pressed &&
                previousGamePadState.DPad.Down == ButtonState.Released) ||
               (currentGamePadState.ThumbSticks.Left.Y < 0 &&
                previousGamePadState.ThumbSticks.Left.Y >= 0))
            {
                selected++;
            }

            if (selected < 0)
                selected = entries.Length - 1;
            if (selected == entries.Length)
                selected = 0;

            if (Input.WasPressed(0, Buttons.Start, Keys.Enter) ||
                (Input.WasPressed(0, Buttons.A, Keys.Space)))
            {
                switch (selected)
                {
                    case 0: //Start Game
                        {
                            if (GameManager.ContainsState(OurGame.PlayingState.Value))
                                GameManager.PopState();
                            else
                            {
                                GameManager.ChangeState(OurGame.PlayingState.Value);
                                OurGame.PlayingState.StartGame();
                            }
                            break;
                        }
                    case 1: //Options Menu
                        {
                            GameManager.PushState(OurGame.OptionsMenuState.Value);
                            break;
                        }
                    case 2: //High Scores
                        {
                            //GameManager.PushState(OurGame.HighScoresState.Value);
                            //OurGame.HighScoresState.AlwaysDisplay = true;
                            break;
                        }
                    case 3: //Exit
                        {
                            GameManager.ChangeState(OurGame.TitleIntroState.Value);
                            break;
                        }
                }
            }

            previousGamePadState = currentGamePadState;
            currentGamePadState = Input.GamePads[0];


            //options menu
            if (Input.WasPressed(0, Buttons.Y, Keys.O))
                GameManager.PushState(OurGame.OptionsMenuState.Value);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 pos = new Vector2((GraphicsDevice.Viewport.Width - texture.Width) / 2,
                (GraphicsDevice.Viewport.Height - texture.Height) / 2);
            Vector2 position = new Vector2(pos.X + 140, pos.Y + texture.Height / 2);

            OurGame.SpriteBatch.Begin();
            OurGame.SpriteBatch.Draw(texture, pos, Color.White);

            for (int i = 0; i < entries.Length; i++)
            {
                Color color;
                float scale;

                if (i == selected)
                {
                    // The selected entry is yellow, and has an animating size.
                    double time = gameTime.TotalGameTime.TotalSeconds;

                    float pulsate = (float)Math.Sin(time * 12) + 1;

                    color = Color.White;
                    scale = 1 + pulsate * 0.05f;
                }
                else
                {
                    // Other entries are white.
                    color = Color.Blue;
                    scale = 1;
                }
                // Draw text, centered on the middle of each line.
                Vector2 origin = new Vector2(0, font.LineSpacing / 2);
                Vector2 shadowPosition = new Vector2(position.X - 2, position.Y - 2);

                //Draw Shadow
                OurGame.SpriteBatch.DrawString(font, entries[i],
                    shadowPosition, Color.Black, 0, origin, scale, SpriteEffects.None, 0);
                //Draw Text
                OurGame.SpriteBatch.DrawString(font, entries[i],
                    position, color, 0, origin, scale, SpriteEffects.None, 0);

                position.Y += font.LineSpacing;
            }
            OurGame.SpriteBatch.End();

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
            font = Content.Load<SpriteFont>(@"Fonts\menu");

            base.LoadContent();

        }
    }
}
