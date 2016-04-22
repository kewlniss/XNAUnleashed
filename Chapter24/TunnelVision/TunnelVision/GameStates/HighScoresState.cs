using System;
using System.IO;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Microsoft.Xna.Framework.Storage;

namespace TunnelVision
{
    public sealed class HighScoresState : BaseGameState, IHighScoresState
    {
        [Serializable]
        public struct HighScoreData
        {
            public string[] PlayerName;
            public int[] Score;
            public int[] Level;

            public int Count;

            public HighScoreData(int count)
            {
                PlayerName = new string[count];
                Score = new int[count];
                Level = new int[count];

                Count = count;
            }
        }

        private readonly string highScoresFilename = "highscores.lst";
        private readonly string containerName = "TunnelVision";

        private Texture2D texture;
        private SpriteFont font;

        private StorageDevice storageDevice;
        private HighScoreData entries;
        private StorageContainer container;
        private IAsyncResult result;
        private bool signingIn = false;
        private bool needToLoadHighScores = false;
        private bool needToSaveHighScores = false;
        private bool newHighScore = false;

        private bool alwaysDisplay = false;

        public HighScoresState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IHighScoresState), this);
        }

        protected override void LoadContent()
        {
            texture = Content.Load<Texture2D>(@"Textures\highscores");
            font = Content.Load<SpriteFont>(@"Fonts\menu");

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            texture = null;

            if (storageDevice != null)
            {
                //Save out our highscores to disk
                if (container != null)
                {
                    container.Dispose();
                    container = null;
                }
            }

            base.UnloadContent();
        }

        protected override void StateChanged(object sender, EventArgs e)
        {
            base.StateChanged(sender, e);

            if (GameManager.State == this.Value)
            {
                signingIn = false;
                newHighScore = false;
                alwaysDisplay = false;

                //Load high scores
                //gets stored in entries
                if (entries.PlayerName == null)
                    LoadHighScores();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (signingIn)
            {
                if (!Guide.IsVisible)
                    Guide.ShowSignIn(1, false);

                return;
            }

            //If high scores aren't being displayed, display them
            //if they are being displayed, hide them
            if (Input.WasPressed(0, Buttons.Back, Keys.Escape) ||
                (Input.WasPressed(0, Buttons.Start, Keys.Enter) ||
                (Input.WasPressed(0, Buttons.A, Keys.Space))))
            {
                GameManager.PopState();
            }

            if ((result != null) && (result.IsCompleted))
            {
                if (needToSaveHighScores)
                {
                    needToSaveHighScores = false;
                    FinishSavingHighScores();
                }

                if (needToLoadHighScores)
                {
                    needToLoadHighScores = false;
                    FinishLoadingHighScores();
                }
            }

            //Need this if first time state is called we have never loaded high scores
            if (newHighScore && entries.PlayerName != null)
            {
                SaveHighScore();
                newHighScore = false;
            }


            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (AlwaysDisplay || newHighScore)
            {
                Vector2 pos = new Vector2(
                    (GraphicsDevice.Viewport.Width - texture.Width) / 2,
                    (GraphicsDevice.Viewport.Height - texture.Height) / 2);

                Vector2 position =
                    new Vector2(pos.X + 90, pos.Y + texture.Height / 2 - 50);

                Vector2 origin = new Vector2(0, font.LineSpacing / 2);

                OurGame.SpriteBatch.Begin();
                OurGame.SpriteBatch.Draw(texture, pos, Color.White);

                if (entries.PlayerName != null)
                {
                    for (int i = 0; i < entries.Count; i++)
                    {
                        // Draw text, centered on the middle of each line.
                        Vector2 scorePosition = new Vector2(position.X + 150, position.Y);
                        Vector2 levelPosition =
                            new Vector2(scorePosition.X + 150, scorePosition.Y);

                        Vector2 shadowPosition =
                            new Vector2(position.X - 1, position.Y - 1);
                        Vector2 shadowScorePosition =
                            new Vector2(scorePosition.X - 1, scorePosition.Y - 1);
                        Vector2 shadowLevelPosition =
                            new Vector2(levelPosition.X - 1, levelPosition.Y - 1);

                        //Draw Name Shadow
                        OurGame.SpriteBatch.DrawString(font, entries.PlayerName[i],
                            shadowPosition, Color.Black, 0, origin, .5f,
                            SpriteEffects.None, 0);
                        //Draw Name
                        OurGame.SpriteBatch.DrawString(font, entries.PlayerName[i],
                            position, Color.Blue, 0, origin, 0.5f, SpriteEffects.None, 0);

                        //Draw Score Shadow
                        OurGame.SpriteBatch.DrawString(font, entries.Score[i].ToString(),
                            shadowScorePosition, Color.Black, 0, origin, .5f,
                            SpriteEffects.None, 0);
                        //Draw Score
                        OurGame.SpriteBatch.DrawString(font, entries.Score[i].ToString(),
                            scorePosition, Color.Blue, 0, origin, 0.5f,
                            SpriteEffects.None, 0);

                        //Draw Level Shadow
                        OurGame.SpriteBatch.DrawString(font, entries.Level[i].ToString(),
                            shadowLevelPosition, Color.Black, 0, origin, .5f,
                            SpriteEffects.None, 0);
                        //Draw Level
                        OurGame.SpriteBatch.DrawString(font, entries.Level[i].ToString(),
                            levelPosition, Color.Blue, 0, origin, 0.5f,
                            SpriteEffects.None, 0);

                        position.Y += font.LineSpacing;
                    }
                }
                else
                {
                    Vector2 shadowPosition =
                        new Vector2(position.X - 1, position.Y - 1);
                    //Draw Name Shadow
                    OurGame.SpriteBatch.DrawString(font, "Loading ...",
                        shadowPosition, Color.Black, 0, origin, .5f,
                        SpriteEffects.None, 0);
                    //Draw Name
                    OurGame.SpriteBatch.DrawString(font, "Loading ...",
                        position, Color.Blue, 0, origin, 0.5f, SpriteEffects.None, 0);
                }
                OurGame.SpriteBatch.End();
            }
            else //No high score was saved, and AlwaysDisplay flag is false
            {
                //no high score was saved
                //no need to toture player that
                //they didn't get on high score list
                GameManager.PopState();
            }

            base.Draw(gameTime);
        }

        private void LoadHighScores()
        {
            if ((result == null) || (result.IsCompleted))
            {
                result = Guide.BeginShowStorageDeviceSelector(null, null);
            }

            needToLoadHighScores = true;
        }

        private void SaveHighScores()
        {
            if ((result == null) || (result.IsCompleted))
            {
                result = Guide.BeginShowStorageDeviceSelector(null, null);
            }

            needToSaveHighScores = true;
        }

        private void FinishLoadingHighScores()
        {
            storageDevice = Guide.EndShowStorageDeviceSelector(result);
            if (storageDevice.IsConnected)
            {
                //storage device is connected, open container and finish saving the high scores
                if (container == null)
                    container = storageDevice.OpenContainer(containerName);

                // Get the path of the save game
                string fullpath = Path.Combine(container.Path, highScoresFilename);

                if (!File.Exists(fullpath))
                {
                    InitializeDefaultHighScores();
                    return;
                }

                // Open the file, creating it if necessary
                FileStream stream = File.Open(fullpath, FileMode.Open, FileAccess.Read);
                try
                {
                    // Convert the object to XML data and put it in the stream
                    XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
                    entries = (HighScoreData)serializer.Deserialize(stream);
                }
                finally
                {
                    // Close the file
                    stream.Close();
                }
            }
            else
                LoadHighScores();
        }

        private void FinishSavingHighScores()
        {
            storageDevice = Guide.EndShowStorageDeviceSelector(result);
            if (storageDevice.IsConnected)
            {
                //storage device is connected, open container and finish saving the high scores
                if (container == null)
                    container = storageDevice.OpenContainer(containerName);

                // Get the path of the save game
                string fullpath = Path.Combine(container.Path, highScoresFilename);

                // Open the file, creating it if necessary
                FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate);
                try
                {
                    // Convert the object to XML data and put it in the stream
                    XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
                    serializer.Serialize(stream, entries);
                }
                finally
                {
                    // Close the file
                    stream.Close();

                    //Since we saved, we will also dispose the container
                    //so it will stick in case the console is turned off
                    //before the game exits
                    if (container != null)
                    {
                        container.Dispose();
                        container = null;
                    }
                }
            }
            else
                SaveHighScores();
        }


        public void SaveHighScore()
        {
            if (entries.PlayerName == null)
            {
                newHighScore = true;
                return;
            }

            int scoreIndex = -1;
            for (int i = 0; i < entries.Count; i++)
            {
                if (OurGame.PlayingState.Score > entries.Score[i])
                {
                    scoreIndex = i;
                    break;
                }
            }

            if (scoreIndex > -1)
            {
                AlwaysDisplay = true;  //if a high score was obtained, always display

                if (SignedInGamer.SignedInGamers.Count > 0)
                {
                    //New high score found ... do swaps
                    for (int i = entries.Count - 1; i > scoreIndex; i--)
                    {
                        entries.PlayerName[i] = entries.PlayerName[i - 1];
                        entries.Score[i] = entries.Score[i - 1];
                        entries.Level[i] = entries.Level[i - 1];
                    }

                    entries.PlayerName[scoreIndex] = Gamer.SignedInGamers[0].Gamertag; //Retrieve User Name Here
                    entries.Score[scoreIndex] = OurGame.PlayingState.Score;
                    entries.Level[scoreIndex] = OurGame.PlayingState.CurrentLevel + 1;

                    SaveHighScores();
                }
                else
                {
                    //register to be notified when gamer signs in
                    SignedInGamer.SignedIn +=
                        new EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);

                    signingIn = true;
                    Guide.ShowSignIn(1, false);

                    return;
                }
            }
            else
                newHighScore = false;

            //We no longer need to be notified
            SignedInGamer.SignedIn -= new EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);
        }

        private void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
        {
            SaveHighScore();
        }

        private void InitializeDefaultHighScores()
        {
            // Create the data to save
            entries = new HighScoreData(5);
            entries.PlayerName[0] = "Neil";
            entries.Level[0] = 10;
            entries.Score[0] = 200500;

            entries.PlayerName[1] = "Chris";
            entries.Level[1] = 10;
            entries.Score[1] = 187000;

            entries.PlayerName[2] = "Mark";
            entries.Level[2] = 9;
            entries.Score[2] = 113300;

            entries.PlayerName[3] = "Cindy";
            entries.Level[3] = 7;
            entries.Score[3] = 400; //95100;

            entries.PlayerName[4] = "Sam";
            entries.Level[4] = 1;
            entries.Score[4] = 100; //1000;
        }

        public bool AlwaysDisplay
        {
            get { return (alwaysDisplay); }
            set { alwaysDisplay = value; }
        }
    }
}
