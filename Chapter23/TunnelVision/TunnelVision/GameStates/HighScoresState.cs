using System;
using System.Xml.Serialization;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;


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
        }

        public override void Initialize()
        {

            base.Initialize();
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

            if (newHighScore && entries.PlayerName != null)
            {
                SaveHighScore();
                newHighScore = false;
            }

            base.Update(gameTime);
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
                    newHighScore = true;

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
            entries.Score[3] = 95100;

            entries.PlayerName[4] = "Sam";
            entries.Level[4] = 1;
            entries.Score[4] = 1000;
        }

        public bool AlwaysDisplay
        {
            get { return (alwaysDisplay); }
            set { alwaysDisplay = value; }
        }

    }
}