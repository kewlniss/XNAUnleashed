using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

using XELibrary;

namespace SimpleGame
{
public class ExplosionManager
        : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Dictionary<int, Explosion> explosions =
            new Dictionary<int, Explosion>(5); 
        private CelAnimationManager cam;
        private SpriteBatch spriteBatch;
        private SimpleGame simpleGame;

        public ExplosionManager(Game game) : this(game, 0) { }
        public ExplosionManager(Game game, int maxExplosions) : base(game)
        {
            simpleGame = (SimpleGame)game;

            cam = (CelAnimationManager)game.Services.GetService(
                                        typeof(ICelAnimationManager));
            if (maxExplosions > 0)
                SetMaxNumberOfExplosions(maxExplosions);
        }

        public void Load(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        public override void Update(GameTime gameTime)
        {
            if (simpleGame.State == SimpleGame.GameState.Scene)
            {
                //Check For Explosions 
                int markForDeletion = -1;
                foreach (KeyValuePair<int, Explosion> explosion in explosions)
                {
                    //have we been playing our explosion for over a second?
                    if (gameTime.TotalGameTime.TotalMilliseconds >
                        explosion.Value.TimeCreated + 100)
                    {
                        markForDeletion = explosion.Key;
                        break;
                    }
                }

                if (explosions.ContainsKey(markForDeletion))
                    explosions.Remove(markForDeletion);
            }

            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            //add our explosions
            cam.AddAnimation("explosion", "explode_1", new CelCount(4, 4), 16);
            cam.AddAnimation("explosion2", "explode_1", new CelCount(4, 4), 16);
            cam.AddAnimation("explosion3", "explode_3", new CelCount(4, 4), 12);
            cam.AddAnimation("explosion4", "explode_4", new CelCount(4, 4), 20);
            cam.AddAnimation("explosion5", "explode_3", new CelCount(4, 4), 12);
            cam.AddAnimation("explosion6", "explode_4", new CelCount(4, 4), 20);

            cam.AddAnimation("bigexplosion", "bigexplosion", new CelCount(4, 4), 18);
        }

        public override void Draw(GameTime gameTime)
        {
            switch (simpleGame.State)
            {
                case SimpleGame.GameState.Scene:
                    {
                        foreach (Explosion explosion in explosions.Values)
                        {
                            cam.Draw(gameTime, "explosion4", spriteBatch,
                                explosion.Position);
                        }
                        break;
                    }
                case SimpleGame.GameState.StartMenu:
                    {
                        //we can add our explosions to make our title page pop
                        cam.Draw(gameTime, "explosion", spriteBatch,
                            new Vector2(32, 32));
                        cam.Draw(gameTime, "explosion2", spriteBatch,
                            new Vector2(40, 40));
                        cam.Draw(gameTime, "explosion3", spriteBatch,
                            new Vector2(64, 32));
                        cam.Draw(gameTime, "explosion4", spriteBatch,
                            new Vector2(64, 64));
                        cam.Draw(gameTime, "explosion5", spriteBatch,
                            new Vector2(28, 40));
                        cam.Draw(gameTime, "explosion6", spriteBatch,
                            new Vector2(40, 64));

                        cam.Draw(gameTime, "explosion", spriteBatch,
                            new Vector2(432, 32));
                        cam.Draw(gameTime, "explosion2", spriteBatch,
                            new Vector2(440, 40));
                        cam.Draw(gameTime, "explosion3", spriteBatch,
                            new Vector2(464, 32));
                        cam.Draw(gameTime, "explosion4", spriteBatch,
                            new Vector2(464, 64));
                        cam.Draw(gameTime, "explosion5", spriteBatch,
                            new Vector2(428, 40));
                        cam.Draw(gameTime, "explosion6", spriteBatch,
                            new Vector2(440, 64));

                        cam.Draw(gameTime, "bigexplosion", spriteBatch,
                            new Vector2(250, 330));
                        break;
                    }
            }
            base.Draw(gameTime);
        }

        public void StartExplosion(int explosionKey, Vector2 position,
            double time)
        {
            explosions.Add(explosionKey, new Explosion(position, time));
        }

        public void SetMaxNumberOfExplosions(int maxExplosions)
        {
            explosions = new Dictionary<int, Explosion>(maxExplosions);
        }
    }

    public class Explosion
    {
        public double TimeCreated;
        public Vector2 Position;

        public Explosion(Vector2 position, double timeCreated)
        {
            Position = position;
            TimeCreated = timeCreated;
        }
    }

}