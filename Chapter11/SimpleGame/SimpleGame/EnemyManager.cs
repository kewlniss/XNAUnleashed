using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

using XELibrary; //for cel animation manager

namespace SimpleGame
{
    public class EnemyManager
        : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private readonly int maxXPosition = 800;
        private readonly int minXPosition = 650;
        private readonly float collisionDistance = 64.0f;
        private int yPosition = 350;
        private int xPosition = 650;

        private Enemy[] enemies;
        private int totalEnemies;
        private int enemiesThisLevel;
        private int maxEnemies;

        private SpriteBatch spriteBatch;
        private Random rand;
        private CelAnimationManager cam;

        public bool EnemiesExist
        {
            get { return (totalEnemies < enemiesThisLevel); }
        }

        public EnemyManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            rand = new Random();

            cam = (CelAnimationManager)game.Services.GetService(
                typeof(ICelAnimationManager));
        }

        public void Load(SpriteBatch spriteBatch, int maxEnemies,
            int enemiesThisLevel, float speed)
        {
            this.spriteBatch = spriteBatch;

            this.enemiesThisLevel = enemiesThisLevel;
            this.totalEnemies = 0;
            this.maxEnemies = maxEnemies;

            Vector2 position = new Vector2();
            enemies = new Enemy[maxEnemies];
            for (int i = 0; i < maxEnemies; i++)
            {
                position.X = GetNextPosition(); //off screen
                position.Y = yPosition;

                enemies[i] = new Enemy();
                enemies[i].Active = true;
                enemies[i].Position = position;
                enemies[i].Velocity = new Vector2(-speed, 0.0f);
            }
        }

        public void SetPause(bool paused)
        {
            cam.ToggleAnimation("robot", paused);
        }

        public void ToggleAnimation()
        {
            cam.ToggleAnimation("robot");
        }

        private float GetNextPosition()
        {
            xPosition += rand.Next(50, 100);
            if (xPosition > maxXPosition)
                xPosition = minXPosition;
            return (xPosition);
        }

        protected override void LoadContent()
        {
            cam.AddAnimation("robot", "robot", new CelCount(4, 4), 16);

        }

        public int CollidedWithPlayer(Vector2 playerPosition)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                Enemy enemy = enemies[i];
                if (enemy.Active)
                {
                    float distance = (playerPosition - enemy.Position).Length();
                    //within collision distance?
                    if (distance < collisionDistance)
                        return (i);
                }
            }
            return (-1);
        }

        public Vector2 Die(int enemyIndex)
        {
            Enemy enemy = enemies[enemyIndex];
            enemy.Active = false;
            Vector2 oldPosition = enemy.Position;

            if (totalEnemies + maxEnemies < enemiesThisLevel)
            {
                Vector2 position = new Vector2();
                position.X = GetNextPosition(); //off screen
                position.Y = yPosition;
                enemy.Position = position;
                enemy.Active = true;
            }
            totalEnemies++;

            return (oldPosition);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = 0; i < enemies.Length; i++)
            {
                Enemy enemy = enemies[i];
                if (enemy.Active)
                {
                    enemy.Position += (enemy.Velocity * elapsed);
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = 0; i < enemies.Length; i++)
            {
                Enemy enemy = enemies[i];
                if (enemy.Active)
                    cam.Draw(gameTime, "robot", spriteBatch, enemy.Position);
            }

            base.Draw(gameTime);
        }
    }

    public class Enemy
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool Active;
    }
}