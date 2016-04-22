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
    public sealed class StartLevelState : BaseGameState, IStartLevelState
    {
        private bool demoMode = true;
        private bool displayedDemoDialog = false;

        private DateTime levelLoadTime;
        private readonly int loadSoundTime = 2500;

        private string levelText = "LEVEL";
        private string currentLevel;

        private Vector2 levelTextPosition;
        private Vector2 levelTextShadowPosition;
        private Vector2 levelNumberPosition;
        private Vector2 levelNumberShadowPosition;


        public StartLevelState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IStartLevelState), this);
        }

        protected override void StateChanged(object sender, EventArgs e)
        {
            base.StateChanged(sender, e);

            bool startingLevel = true;

            if (GameManager.State == this.Value)
            {
                if (demoMode && !displayedDemoDialog)
                {
                    //We could set properties on our YesNoDialog
                    //so it could have a custom message and custom
                    //Yes / No buttons ...
                    //YesNoDialogState.YesCaption = “Of course!”;
                    GameManager.PushState(OurGame.YesNoDialogState.Value);
                    this.Visible = true;
                    displayedDemoDialog = true;
                    startingLevel = false;
                }
            }

            if (startingLevel)
            {
                //play sound

                levelLoadTime = DateTime.Now;

                currentLevel = (OurGame.PlayingState.CurrentLevel + 1).ToString();

                Vector2 viewport = new Vector2(GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height);
                Vector2 levelTextLength = OurGame.Font.MeasureString(levelText);
                Vector2 levelNumberLength = OurGame.Font.MeasureString(currentLevel);
                levelTextShadowPosition = (viewport - levelTextLength * 3) / 2;
                levelNumberShadowPosition = (viewport - levelNumberLength * 3) / 2;
                levelNumberShadowPosition.Y += OurGame.Font.LineSpacing * 3;
                levelTextPosition.X = levelTextShadowPosition.X + 2;
                levelTextPosition.Y = levelTextShadowPosition.Y + 2;
                levelNumberPosition.X = levelNumberShadowPosition.X + 2;
                levelNumberPosition.Y = levelNumberShadowPosition.Y + 2;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (DateTime.Now > levelLoadTime + new TimeSpan(0, 0, 0, 0, loadSoundTime))
            {
                //stop sound

                // change state to playing
                GameManager.ChangeState(OurGame.PlayingState.Value);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            OurGame.SpriteBatch.Begin();
            OurGame.SpriteBatch.DrawString(OurGame.Font, levelText,
                levelTextShadowPosition, Color.Yellow, 0, Vector2.Zero, 3.0f,
                SpriteEffects.None, 0);
            OurGame.SpriteBatch.DrawString(OurGame.Font, levelText,
                levelTextPosition, Color.Red, 0, Vector2.Zero, 3.0f,
                SpriteEffects.None, 0);
            OurGame.SpriteBatch.DrawString(OurGame.Font, currentLevel,
                levelNumberShadowPosition, Color.Yellow, 0, Vector2.Zero, 3.0f,
                SpriteEffects.None, 0);
            OurGame.SpriteBatch.DrawString(OurGame.Font, currentLevel,
                levelNumberPosition, Color.Red, 0, Vector2.Zero, 3.0f,
                SpriteEffects.None, 0);
            OurGame.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
