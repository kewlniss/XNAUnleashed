using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

using XELibrary;

namespace VisualizationDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private const int width = 240;
        private const int height = 320;

        private SpriteFont font;

        private Texture2D art;
        private Texture2D noArt;

        private string songName = string.Empty;
        private string artistName = string.Empty;
        private Playlist playlist = null;

        private Vector2 songPosShadow = new Vector2(0, 230);
        private Vector2 songPos = new Vector2(1, 231);

        private Vector2 artistPosShadow = Vector2.Zero;
        private Vector2 artistPos = Vector2.Zero;

        private InputHandler input;
        private FPS fps;

        private VisualizationData visualizationData;

        private Color color = Color.Black;
        private Color[] pixelData;
        private Texture2D visualization;
        private Vector2 pos = Vector2.Zero;
        private int x, y;
        private int maxData;
        private int baseline;

        private int freqencyValue;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = height;
            graphics.PreferredBackBufferWidth = width;

            input = new InputHandler(this);
            Components.Add(input);

            fps = new FPS(this);
            Components.Add(fps);

            MediaPlayer.ActiveSongChanged +=
                new EventHandler(MediaPlayer_ActiveSongChanged);

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            noArt = Content.Load<Texture2D>(@"Textures\NoArt");
            font = Content.Load<SpriteFont>(@"Fonts\Arial");

            visualizationData = new VisualizationData();

            MediaPlayer.IsVisualizationEnabled = true;

            if (MediaPlayer.State == MediaState.Playing)
            {
                //the user is playing their own music - don't reset it
                //just retreive the current playing song and artist and art
                MediaPlayer_ActiveSongChanged(this, null);

                return;
            }

            ICollection<MediaSource> mediaSources = MediaSource.GetAvailableMediaSources();
            MediaLibrary mediaLib = null;

            foreach (MediaSource ms in mediaSources)
            {
                mediaLib = new MediaLibrary(ms);
            }

            if (mediaLib != null)
            {
                for (int i = 0; i < mediaLib.Playlists.Count; i++)
                {

                    if (mediaLib.Playlists[i].Name == "Zune Gems")
                    {
                        playlist = mediaLib.Playlists[i];
                        break;
                    }
                }

                if (playlist == null)
                    playlist = mediaLib.Playlists[0]; // just grab first one
            }

            //If can't find the playlist, just exit.
            if (playlist == null)
                Exit();

            //Assumes at least one song is actually in the playlist
            MediaPlayer.Play(playlist.Songs);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (input.ButtonHandler.WasButtonPressed(0, Buttons.DPadRight)
#if !ZUNE
                || input.KeyboardState.WasKeyPressed(Keys.Right)
#endif
                )
            {
                MediaPlayer.MoveNext();
            }
            else if (input.ButtonHandler.WasButtonPressed(0, Buttons.DPadLeft)
#if !ZUNE
                 || input.KeyboardState.WasKeyPressed(Keys.Left)
#endif
                )
            {
                MediaPlayer.MovePrevious();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (art != null)
                spriteBatch.Draw(art, pos, Color.White);
            else
                spriteBatch.Draw(noArt, pos, Color.White);

            try
            {
                spriteBatch.DrawString(font, songName, songPosShadow, Color.Gray);
                spriteBatch.DrawString(font, songName, songPos, Color.White);
            }
            catch
            {
                spriteBatch.DrawString(font, "Song: ???",
                    songPosShadow, Color.Gray);
                spriteBatch.DrawString(font, "Song: ???",
                    songPos, Color.White);
            }

            artistPosShadow.Y = songPosShadow.Y + font.LineSpacing;
            artistPos.Y = songPos.Y + font.LineSpacing;

            try
            {
                spriteBatch.DrawString(font, artistName, artistPosShadow, Color.Gray);
                spriteBatch.DrawString(font, artistName, artistPos, Color.White);
            }
            catch
            {
                spriteBatch.DrawString(font, "Artist: ???",
                    artistPosShadow, Color.Gray);
                spriteBatch.DrawString(font, "Artist: ???",
                    artistPos, Color.White);
            }


            if (MediaPlayer.IsVisualizationEnabled && !MediaPlayer.IsMuted)
            {
                CreateVisualizationTexture(ref visualization);

                spriteBatch.Draw(visualization, pos, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private Color[] blankData = new Color[width * height];

        private void CreateVisualizationTexture(ref Texture2D texture)
        {
            MediaPlayer.GetVisualizationData(visualizationData);

            texture = new Texture2D(GraphicsDevice, width, height, 1,
                TextureUsage.None, SurfaceFormat.Color);

            pixelData = new Color[width * height];

            //there are 256 total frequency/sample values, but we only have 240 pixels
            //so we are going to chop off 8 values on both ends
            maxData = width;

            //Display Frequency (Pitch) Data
            for (x = 0; x < maxData; x++)
            {
                freqencyValue = (int)(visualizationData.Frequencies[x + 8] * 60);

                for (y = height - 1; y > height - freqencyValue; y--)
                {
                    if (y > height - freqencyValue + 4)
                    {
                        color.R = (byte)(55 + (height - y) * 1);
                        color.G = (byte)(65 - (height - y) * 1);
                        color.B = (byte)(155 - (height - y) * 2);


                        //Cool spiral look
                        //color.R = (byte)((x * y * 3) * .25);
                        //color.G = (byte)((x * y * 3) * .5);
                        //color.B = (byte)((x * y * -3));
                    }
                    else
                    {
                        color.R = (byte)(y);
                        color.G = (byte)(y);
                        color.B = (byte)(y);
                    }

                    pixelData[(y * width) + x] = color;
                }
            }

            //Display Sample (Volume) Data
            //now overwrite the pixel data with the waveform / sample data (equates to volume)
            for (x = 0; x < maxData; x++)
            {
                baseline = (height - 50);
                y = baseline + (int)(visualizationData.Samples[x + 8] * 25);

                //plot base line
                pixelData[(baseline * width) + x] = Color.DarkGray;

                //now plot actual wave
                pixelData[(y * width) + x] = Color.White;
            }


            texture.SetData(pixelData);
        }

        private void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
        {
            if (art != null)
                art.Dispose();

            art = MediaPlayer.Queue.ActiveSong.Album.GetAlbumArt(Services);
            //if (art == null)
            //    art = MediaPlayer.Queue.ActiveSong.Album.GetThumbnail(Services);

            songName = MediaPlayer.Queue.ActiveSong.Name;
            artistName = MediaPlayer.Queue.ActiveSong.Artist.Name;

            if (MediaPlayer.State != MediaState.Playing)
                MediaPlayer.Play(playlist.Songs, MediaPlayer.Queue.ActiveSongIndex);
        }
    }
}
