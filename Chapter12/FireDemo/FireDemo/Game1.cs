using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using XELibrary;

namespace FireDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Texture2D fire;
        private InputHandler input;
        private FPS fps;
        private uint[] pixelData;
        private uint[] firePalette;
        private Dictionary<uint, uint> firePaletteData;
        private Random rand = new Random();

        private int scale = 1;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

#if ZUNE
            graphics.PreferredBackBufferHeight = 320;
            graphics.PreferredBackBufferWidth = 240;

            // Frame rate is 30 fps by default for Zune.
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 30.0);
#else
            graphics.PreferredBackBufferHeight = 256;
            graphics.PreferredBackBufferWidth = 512;
#endif

            fps = new FPS(this);
            Components.Add(fps);

            input = new InputHandler(this);
            Components.Add(input);
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

            //256x1 image
            Texture2D fireTexture = Content.Load<Texture2D>(@"Textures\FireGrade");

            //initialize our array to store the palette info
            firePaletteData = new Dictionary<uint, uint>(fireTexture.Width * fireTexture.Height);
            firePalette = new uint[fireTexture.Width * fireTexture.Height];

            //storing image data in our fireTexture
            fireTexture.GetData<uint>(firePalette);

            //load up our data dictionary with the actual palette color and the index
            for (uint i = 0; i < firePalette.Length; i++)
            {
                if (!firePaletteData.ContainsKey(firePalette[i]))
                    firePaletteData.Add(firePalette[i], i);
            }

            //now we can create our fire texture
#if ZUNE
            fire = CreateFireTexture(32, 32);
#else
            fire = CreateFireTexture(128, 128);
#endif
        }

        private Texture2D CreateFireTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(
                GraphicsDevice, width, height, true, SurfaceFormat.Color);

            int pixelCount = width * height;
            pixelData = new uint[pixelCount];

            for (uint i = 0; i < pixelCount; i++)
                pixelData[i] = Color.Black.PackedValue;

            //set bottom 16 rows to fiery colors
            for (int y = height - 1; y > height - 16; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    pixelData[(y * width) + x] = GetRandomFireColor();
                }
            }

            texture.SetData(pixelData);

            return (texture);
        }

        private uint GetRandomFireColor()
        {
            return (firePalette[rand.Next(0, 256)]);
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
            UpdateFire();

            base.Update(gameTime);
        }

        private void UpdateFire()
        {
            int height = fire.Height;
            int width = fire.Width;

            uint left, right, bottom, top;
            uint colorIndex;
            int xRight, xLeft, yBelow, yAbove;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    xRight = x + 1;
                    yBelow = y + 1;
                    xLeft = x - 1;
                    yAbove = y - 1;

                    //make sure our values are within range
                    if (xRight >= width)
                        xRight = 0;
                    if (yBelow >= height)
                        yBelow = height - 1;

                    if (xLeft < 0)
                        xLeft = width - 1;
                    if (yAbove < 0)
                        yAbove = 0;

                    //we need to get uint value of each surrounding color
                    right = pixelData[(y * width) + xRight];
                    left = pixelData[(y * width) + xLeft];
                    bottom = pixelData[(yBelow * height) + x];
                    top = pixelData[(yAbove * height) + x];

                    //we now have the uint value of the colors stored 
                    //we need to find our palette value for this color
                    //and do an average and reset the color
                    colorIndex = (firePaletteData[right] + firePaletteData[left] +
                        firePaletteData[top] + firePaletteData[bottom]) / 4;

                    colorIndex -= 3; //could make a random number

                    if (colorIndex > 255) //went “negative” (unsigned)
                        colorIndex = 0;

                    //finally, we can set this pixel to our new color
                    pixelData[(yAbove * width) + x] = firePalette[colorIndex];
                }
            }

            //now, it is time to animate our fire            
            AnimateFire(width, height);
        }

        private void AnimateFire(int width, int height)
        {
            int newColorIndex;

            //we could work with just one pixel at a time,
            //but we will work with five for a broader flame
            for (int x = 0; x < width - 5; x += 5)
            {
                // we are only going to modify the bottom row
                int y = height - 1;

                //we get our palette index for the color we just set on the
                //bottom row we then either add or subtract a substantial 64 to or from
                //that index
                newColorIndex = (int)firePaletteData[pixelData[(y * width) + x]] +
                    (rand.Next(-64, 64));

                //now we make sure our palette index is within range
                if (newColorIndex > 255)
                    newColorIndex = 255;
                if (newColorIndex < 0)
                    newColorIndex = 0;

                //Because we are stepping through our loop by a factor of 5
                //we can set all adjacent rows from here and over 4 additional
                //space and set those pixels to the same color.
                //an interesting effect is to change line 4 (x + 2) with
                //Color.Navy.PackedValue; It makes it look like it has a
                //really hot blue center 
                uint ci = (uint)newColorIndex;
                pixelData[(y * width) + x] = firePalette[ci];
                pixelData[(y * width) + x + 1] = firePalette[ci];
                pixelData[(y * width) + x + 2] = firePalette[ci];
                pixelData[(y * width) + x + 3] = firePalette[ci];
                pixelData[(y * width) + x + 4] = firePalette[ci];
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

#if !ZUNE
            //Need to clear out the texture on the graphics device
            //to keep from getting the following runtime error:
            //The operation was aborted. You may not modify a resource that has been set on a device, or after it has been used within a tiling bracket.
            graphics.GraphicsDevice.Textures[0] = null;
#endif

            fire.SetData<uint>(pixelData);

            int h = graphics.GraphicsDevice.Viewport.Height - fire.Height;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            //left side
            int y = 0;
            for (int i = 0; i < graphics.PreferredBackBufferHeight; i += fire.Height * scale, y++)
            {
                spriteBatch.Draw(fire, new Vector2(fire.Width * scale, i), null,
                    Color.White, MathHelper.PiOver2, Vector2.Zero, scale,
                    (y % 2 == 0) ?
                        SpriteEffects.None
                    :
                        SpriteEffects.FlipHorizontally,
                    0);
            }

            //right side
            y = 0;
            for (int i = 0; i < graphics.PreferredBackBufferHeight; i += fire.Height * scale, y++)
            {
                spriteBatch.Draw(fire, new Vector2(graphics.PreferredBackBufferWidth, i),
                    null, Color.White, MathHelper.PiOver2,
                    Vector2.Zero, scale, (y % 2 == 0) ?
                        SpriteEffects.FlipVertically
                    :
                        SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally,
                    0);
            }

            //bottom row
            int x = 0;
            for (int i = 0; i < graphics.PreferredBackBufferWidth; i += fire.Width * scale, x++)
            {
                spriteBatch.Draw(fire, new Vector2(
                    i, graphics.PreferredBackBufferHeight - fire.Height * scale),
                    null, Color.White, 0, Vector2.Zero, scale,
                    (x % 2 == 0) ?
                        SpriteEffects.None
                    :
                        SpriteEffects.FlipHorizontally,
                    0);
            }

            //top row
            x = 0;
            for (int i = 0; i < graphics.PreferredBackBufferWidth; i += fire.Width * scale, x++)
            {
                spriteBatch.Draw(fire, new Vector2(i, 0), null, Color.White, 0,
                    Vector2.Zero, scale,
                    (x % 2 == 0) ?
                        SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally
                    :
                        SpriteEffects.FlipVertically,
                    0);
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
