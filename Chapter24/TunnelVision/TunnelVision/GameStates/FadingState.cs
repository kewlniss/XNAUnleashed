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
    public sealed class FadingState : BaseGameState, IFadingState
    {
        private Texture2D fadeTexture;
        private float fadeAmount;
        private double fadeStartTime;

        private Color color;

        public Color Color
        {
            get { return (color); }
            set { color = value; }
        }

        public FadingState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IFadingState), this);
        }

        public override void Update(GameTime gameTime)
        {
            if (fadeStartTime == 0)
                fadeStartTime = gameTime.TotalGameTime.TotalMilliseconds;

            fadeAmount += (.25f *(float)gameTime.ElapsedGameTime.TotalSeconds);

            if (gameTime.TotalGameTime.TotalMilliseconds > fadeStartTime+4000)
            {
                //We get here by winning or losing
                //Change State to Intro and push on HighScore
                //It is up to HighScore to show or not depending on
                //if a high score was achieved
                GameManager.ChangeState(OurGame.TitleIntroState.Value);
                //GameManager.PushState(OurGame.HighScoresState.Value);
                //OurGame.HighScoresState.SaveHighScore();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            //GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            Vector4 fadeColor = color.ToVector4();
            fadeColor.W = fadeAmount; //set transparancy

            OurGame.SpriteBatch.Begin();
            OurGame.SpriteBatch.Draw(fadeTexture, Vector2.Zero,
                new Color(fadeColor));
            OurGame.SpriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void StateChanged(object sender, EventArgs e)
        {
            //Set up our initial fading values
            if (GameManager.State == this.Value)
            {
                fadeAmount = 0;
                fadeStartTime = 0;
            }
        }
        protected override void LoadContent()
        {
            fadeTexture = CreateFadeTexture(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            base.LoadContent();
        }

        private Texture2D CreateFadeTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, width, height, false,
                SurfaceFormat.Color);

            int pixelCount = width * height;
            Color[] pixelData = new Color[pixelCount];
            Random rnd = new Random();

            for (int i = 0; i < pixelCount; i++)
            {
                pixelData[i] = Color.White;
            }

            texture.SetData(pixelData);

            return (texture);
        }
        /*
        private void SaveHighScore()
        {
            // Create the data to save
            HighScoreData data = highScoresState.LoadHighScores();

            int scoreIndex = -1;
            for (int i = 0; i < data.Count; i++)
            {
                if (OurGame.PlayingState.Score > data.Score[i])
                {
                    scoreIndex = i;
                    break;
                }
            }

            if (scoreIndex > -1)
            {
                if (SignedInGamer.SignedInGamers.Count > 0)
                {
                    //New high score found ... do swaps
                    for (int i = data.Count - 1; i > scoreIndex; i--)
                    {
                        data.PlayerName[i] = data.PlayerName[i - 1];
                        data.Score[i] = data.Score[i - 1];
                        data.Level[i] = data.Level[i - 1];
                    }

                    data.PlayerName[scoreIndex] = Gamer.SignedInGamers[0].Gamertag; //Retrieve User Name Here

                    data.Score[scoreIndex] = OurGame.PlayingState.Score;
                    data.Level[scoreIndex] = OurGame.PlayingState.CurrentLevel + 1;

                    highScoresState.SaveHighScores(data);
                }
                else
                {
                    SignedInGamer.SignedIn +=
                        new EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);

                    signingIn = true;
                    Guide.ShowSignIn(1, false);

                    //Don't exit this state until we have a highscore added
                    return;
                }
            }

            //Once we are done fading, change back to title screen.
            GameManager.ChangeState(OurGame.TitleIntroState.Value);

            //Unregister for the event, user is already signed in
            SignedInGamer.SignedIn -= new EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);

            //If we stored a high score, display the high scores screen
            if (scoreIndex > -1)
                GameManager.PushState(OurGame.HighScoresState.Value);
        }

        void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
        {
            SaveHighScore();
        }
        */
    }
}
