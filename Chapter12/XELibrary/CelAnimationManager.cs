using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace XELibrary
{
    public interface ICelAnimationManager { }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public sealed class CelAnimationManager
        : Microsoft.Xna.Framework.GameComponent, ICelAnimationManager
    {

        private Dictionary<string, CelAnimation> animations =
            new Dictionary<string, CelAnimation>();
        private Dictionary<string, Texture2D> textures =
            new Dictionary<string, Texture2D>();

        private string contentPath;

        public CelAnimationManager(Game game, string contentPath)
            : base(game)
        {
            this.contentPath = contentPath;

            if (this.contentPath.LastIndexOf('\\') <
                this.contentPath.Length - 1)
            {
                this.contentPath += "\\";
            }

            game.Services.AddService(
                typeof(ICelAnimationManager), this);
        }


        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        public void AddAnimation(string animationKey, string textureName,
                                 CelCount celCount, int framesPerSecond)
        {
            if (!textures.ContainsKey(textureName))
            {
                textures.Add(textureName, Game.Content.Load<Texture2D>(
                    contentPath + textureName));
            }

            int celWidth = (int)(textures[textureName].Width /
                celCount.NumberOfColumns);
            int celHeight = (int)(textures[textureName].Height /
                celCount.NumberOfRows);

            int numberOfCels = celCount.NumberOfColumns *
                celCount.NumberOfRows;

            //we create a cel range by passing in start location of 1,1
            //and end with number of column and rows
            //2,1  =   1,1,2,1  ;    4,2  =  1,1,4,2
            AddAnimation(animationKey, textureName,
                new CelRange(1, 1, celCount.NumberOfColumns,
                celCount.NumberOfRows), celWidth, celHeight,
                numberOfCels, framesPerSecond);
        }

        public void AddAnimation(string animationKey, string textureName,
            CelRange celRange, int celWidth, int celHeight,
            int numberOfCels, int framesPerSecond)
        {
            CelAnimation ca = new CelAnimation(textureName, celRange,
                framesPerSecond);

            if (!textures.ContainsKey(textureName))
            {
                textures.Add(textureName, Game.Content.Load<Texture2D>(
                    contentPath + textureName));
            }

            ca.CelWidth = celWidth;
            ca.CelHeight = celHeight;

            ca.NumberOfCels = numberOfCels;

            ca.CelsPerRow = textures[textureName].Width / celWidth;

            if (animations.ContainsKey(animationKey))
                animations[animationKey] = ca;
            else
                animations.Add(animationKey, ca);
        }

        public void ToggleAnimation(string animationKey)
        {
            if (animations.ContainsKey(animationKey))
            {
                animations[animationKey].Paused =
                    !animations[animationKey].Paused;
            }
        }

        public void ToggleAnimation(string animationKey, bool paused)
        {
            if (animations.ContainsKey(animationKey))
                animations[animationKey].Paused = paused;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            foreach (KeyValuePair<string, CelAnimation> animation in animations)
            {
                CelAnimation ca = animation.Value;

                if (ca.Paused)
                    continue; //no need to update this animation,check next one

                ca.TotalElapsedTime +=
                    (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (ca.TotalElapsedTime > ca.TimePerFrame)
                {
                    ca.Frame++;

                    //min: 0, max: total cels
                    ca.Frame = ca.Frame % (ca.NumberOfCels);

                    //reset our timer
                    ca.TotalElapsedTime -= ca.TimePerFrame;
                }
            }

            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime, string animationKey,
            SpriteBatch batch, Vector2 position)
        {
            Draw(gameTime, animationKey, batch,
                position, Color.White);
        }

        public void Draw(GameTime gameTime, string animationKey,
            SpriteBatch batch, Vector2 position, Color color)
        {
            if (!animations.ContainsKey(animationKey))
                return;

            CelAnimation ca = animations[animationKey];

            //first get our x increase amount
            //(add our offset-1 to our current frame)
            int xincrease = (ca.Frame + ca.CelRange.FirstCelX - 1);
            //now we need to wrap the value so it will loop to the next row
            int xwrapped = xincrease % ca.CelsPerRow;
            //finally we need to take the product of our wrapped value
            //and a cel’s width
            int x = xwrapped * ca.CelWidth;

            //to determine how much we should increase y, we need to look
            //at how much we increased x and do an integer divide
            int yincrease = xincrease / ca.CelsPerRow;
            //now we can take this increase and add it to
            //our Y offset-1 and multiply the sum by our cel height
            int y = (yincrease + ca.CelRange.FirstCelY - 1) * ca.CelHeight;

            Rectangle cel = new Rectangle(x, y, ca.CelWidth, ca.CelHeight);
            batch.Draw(textures[ca.TextureName], position, cel, color);
        }
    }
    public class CelAnimation
    {
        private string textureName;
        private CelRange celRange;
        private int framesPerSecond;
        private float timePerFrame;

        public float TotalElapsedTime = 0.0f;
        public int CelWidth;
        public int CelHeight;
        public int NumberOfCels;
        public int CelsPerRow;
        public int Frame;
        public bool Paused = false;

        public CelAnimation(string textureName, CelRange celRange,
            int framesPerSecond)
        {
            this.textureName = textureName;
            this.celRange = celRange;
            this.framesPerSecond = framesPerSecond;
            this.timePerFrame = 1.0f / (float)framesPerSecond;
            this.Frame = 0;
        }

        public string TextureName
        {
            get { return (textureName); }
        }

        public CelRange CelRange
        {
            get { return (celRange); }
        }

        public int FramesPerSecond
        {
            get { return (framesPerSecond); }
        }

        public float TimePerFrame
        {
            get { return (timePerFrame); }
        }
    }

    public struct CelCount
    {
        public int NumberOfColumns;
        public int NumberOfRows;

        public CelCount(int numberOfColumns, int numberOfRows)
        {
            NumberOfColumns = numberOfColumns;
            NumberOfRows = numberOfRows;
        }
    }

    public struct CelRange
    {
        public int FirstCelX;
        public int FirstCelY;
        public int LastCelX;
        public int LastCelY;

        public CelRange(int firstCelX, int firstCelY, int lastCelX,
            int lastCelY)
        {
            FirstCelX = firstCelX;
            FirstCelY = firstCelY;
            LastCelX = lastCelX;
            LastCelY = lastCelY;
        }
    }
}