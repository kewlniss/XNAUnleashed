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
    public sealed class PlayingState : BaseGameState, IPlayingState
    {
        private MissileManager missileManager;
        private EnemyManager enemyManager;
        private List<Level> Levels;
        private int currentLevel;

        private float totalCreatedEnemies;
        public int TotalCollisions;

        private BoundingSphere playerSphere;

        private Random rand;

        private Texture2D crossHair;

        private Tunnel tunnel;

        private TimeSpan? storedTime;
        private TimeSpan currentLevelTime;
        private DateTime currentLevelStopTime = DateTime.Now;

        private string levelText = string.Empty;
        private Vector2 levelTextShadowPosition;
        private Vector2 levelTextPosition;

        private string enemiesText = string.Empty;
        private Vector2 enemiesTextShadowPosition;
        private Vector2 enemiesTextPosition;

        private string timeText = string.Empty;
        private Vector2 timeTextShadowPosition;
        private Vector2 timeTextPosition;

        private string scoreText = string.Empty;
        private Vector2 scoreTextShadowPosition;
        private Vector2 scoreTextPosition;
        public int score;

        public PlayingState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IPlayingState), this);
            rand = new Random();

            playerSphere = new BoundingSphere(OurGame.Camera.Position, 1.5f);

            tunnel = new Tunnel(Game);
            Game.Components.Add(tunnel);

            missileManager = new MissileManager(Game);
            Game.Components.Add(missileManager);
            missileManager.Enabled = false;
            missileManager.Visible = false;

            enemyManager = new EnemyManager(Game);
            Game.Components.Add(enemyManager);
            enemyManager.Enabled = false;
            enemyManager.Visible = false;

            Levels = new List<Level>(10);
            Levels.Add(new Level(50, 10, 60, 9.0f));
            Levels.Add(new Level(25, 10, 60, 9.0f));
            Levels.Add(new Level(15, 15, 60, 9.0f));
            Levels.Add(new Level(10, 15, 60, 9.0f));
            Levels.Add(new Level(5, 15, 60, 9.0f));
            Levels.Add(new Level(5, 20, 60, 9.0f));
            Levels.Add(new Level(5, 25, 60, 9.0f));
            Levels.Add(new Level(5, 30, 60, 10.0f));
            Levels.Add(new Level(5, 40, 90, 10.0f));
            Levels.Add(new Level(3, 50, 90, 10.0f));

            currentLevel = 0;
            enemyManager.Enemies = new List<Enemy>(Levels[CurrentLevel].Enemies);

            OurGame.Camera.MoveRate = 10;
            OurGame.Camera.SpinRate = 60;
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            currentLevelTime = currentLevelStopTime.Subtract(DateTime.Now);
            if (currentLevelTime.Seconds < 0)
                currentLevelTime = TimeSpan.Zero;

            if (Input.WasPressed(0, Buttons.Back, Keys.Escape))
            {
                storedTime = currentLevelTime;
                GameManager.PushState(OurGame.StartMenuState.Value);
            }

            if (Input.WasPressed(0, Buttons.Start, Keys.Enter))
            {
                storedTime = currentLevelTime;
                GameManager.PushState(OurGame.PausedState.Value);
            }


            if ((Input.WasPressed(0, Buttons.A, Keys.Space)) ||
               (Input.WasPressed(0, Buttons.RightShoulder,
                    Keys.LeftControl)) ||
               Input.GamePads[0].Triggers.Right > 0
                )
            {
                if (missileManager.AddMissile(new Vector3(
                        OurGame.Camera.Position.X,
                        OurGame.Camera.Position.Y - 1,
                        OurGame.Camera.Position.Z + 1
                    ), OurGame.Camera.Target - OurGame.Camera.Position,
                    DateTime.Now))
                {
                    //play sound
                }
            }

            if (enemyManager.Enabled)
            {
                UpdateEnemies(elapsed);

                while (CheckCollisions())
                {
                    //increase score if enemy was hit
                    score += 100; //100 points for each enemy
                }

                //Are we finished with this level?
                if (TotalCollisions == Levels[CurrentLevel].Enemies)
                {
                    if (currentLevelTime.Seconds > 0)
                        score += ((int)currentLevelTime.TotalSeconds * 500);

                    TotalCollisions = 0;
                    currentLevel++;

                    //Are we finished with the game?
                    if (CurrentLevel == Levels.Count)
                    {
                        //You won the game!!!
                        GameManager.PushState(OurGame.WonGameState.Value);
                        currentLevel--; //reset count back
                    }
                    else
                    {
                        StartLevel();
                    }
                }
            }

            levelText = "Level: " + ((int)(CurrentLevel + 1)).ToString();
            timeText = "Time: " + ((int)currentLevelTime.TotalSeconds).ToString();
            enemiesText = "Enemies: " +
                ((int)(Levels[CurrentLevel].Enemies - TotalCollisions)).ToString();
            scoreText = "Score: " + score.ToString();

            base.Update(gameTime);

        }


        public void StartGame()
        {
            SetupGame();
            StartLevel();
        }

        private void SetupGame()
        {
            TotalCollisions = 0;
            currentLevel = 0;
            score = 0;
            currentLevelTime = TimeSpan.Zero;
        }

        public void StartLevel()
        {
            GamePad.SetVibration(0, 0, 0);
            enemyManager.Enemies.Clear();
            totalCreatedEnemies = 0;
            storedTime = null;

            missileManager.Load(Levels[CurrentLevel].Missiles);

            GameManager.PushState(OurGame.StartLevelState.Value);
        }

        private void UpdateEnemies(float elapsed)
        {
            if (totalCreatedEnemies < Levels[CurrentLevel].Enemies)
            {
                if (enemyManager.Enemies.Count < EnemyManager.MAX_ENEMIES)
                {
                    enemyManager.AddEnemy(Levels[CurrentLevel].EnemySpeed);
                    totalCreatedEnemies++;
                }
            }

            for (int ei = 0; ei < enemyManager.Enemies.Count; ei++)
            {
                enemyManager.Enemies[ei].Target = OurGame.Camera.Position;
                enemyManager.Enemies[ei].Move(elapsed);
            }
        }


        public override void Draw(GameTime gameTime)
        {
            missileManager.View = OurGame.Camera.View;
            missileManager.Projection = OurGame.Camera.Projection;

            enemyManager.View = OurGame.Camera.View;
            enemyManager.Projection = OurGame.Camera.Projection;


            OurGame.SpriteBatch.Begin();
            if (OurGame.DisplayCrosshair)
            {
                OurGame.SpriteBatch.Draw(crossHair, new Rectangle(
                    (GraphicsDevice.Viewport.Width - crossHair.Width) / 2,
                    (GraphicsDevice.Viewport.Height - crossHair.Height) / 2,
                    crossHair.Width, crossHair.Height), Color.White);
            }

            if (OurGame.DisplayRadar)
            {
                if (enemyManager.Radar != null)
                    OurGame.SpriteBatch.Draw(enemyManager.Radar,
                        new Rectangle(
                            TitleSafeArea.Left,
                            TitleSafeArea.Bottom - 200,
                            200, 200),
                        new Color(new Vector4(1, 1, 1, .5f)));
            }

            OurGame.SpriteBatch.DrawString(OurGame.Font, levelText,
                levelTextShadowPosition, Color.Black);
            OurGame.SpriteBatch.DrawString(OurGame.Font, levelText,
                levelTextPosition, Color.WhiteSmoke);

            OurGame.SpriteBatch.DrawString(OurGame.Font, enemiesText,
                enemiesTextShadowPosition, Color.Black);
            OurGame.SpriteBatch.DrawString(OurGame.Font, enemiesText,
                enemiesTextPosition, Color.Firebrick);

            OurGame.SpriteBatch.DrawString(OurGame.Font, timeText,
                timeTextShadowPosition, Color.Black);
            OurGame.SpriteBatch.DrawString(OurGame.Font, timeText,
                timeTextPosition, Color.Firebrick);

            OurGame.SpriteBatch.DrawString(OurGame.Font, scoreText,
                scoreTextShadowPosition, Color.Black);
            OurGame.SpriteBatch.DrawString(OurGame.Font, scoreText,
                scoreTextPosition, Color.Firebrick);

            OurGame.SpriteBatch.End();

            tunnel.View = OurGame.Camera.View;
            tunnel.Projection = OurGame.Camera.Projection;


            base.Draw(gameTime);
        }

        protected override void StateChanged(object sender, EventArgs e)
        {
            base.StateChanged(sender, e);

            if (GameManager.State != this.Value)
            {
                Visible = true;
                Enabled = false;
                missileManager.Enabled = false;
                missileManager.Visible = false;
                enemyManager.Enabled = false;
                enemyManager.Visible = false;
                OurGame.Camera.UpdateInput = false;
            }
            else
            {
                missileManager.Enabled = true;
                missileManager.Visible = true;
                enemyManager.Enabled = true;
                enemyManager.Visible = true;
                OurGame.Camera.UpdateInput = true;

                if (storedTime != null)
                    currentLevelStopTime = DateTime.Now + (TimeSpan)storedTime;
                else
                    currentLevelStopTime = DateTime.Now +
                        new TimeSpan(0, 0, Levels[CurrentLevel].Time);
            }
        }

        protected override void LoadContent()
        {
            crossHair = Content.Load<Texture2D>(@"Textures\crosshair");

            levelTextShadowPosition = new Vector2(TitleSafeArea.X, TitleSafeArea.Y);
            levelTextPosition = new Vector2(TitleSafeArea.X + 2.0f, TitleSafeArea.Y + 2.0f);

            enemiesTextShadowPosition = new Vector2(TitleSafeArea.X, TitleSafeArea.Y +
                OurGame.Font.LineSpacing);
            enemiesTextPosition = new Vector2(TitleSafeArea.X + 2.0f, TitleSafeArea.Y +
                OurGame.Font.LineSpacing + 2.0f);

            timeTextShadowPosition = new Vector2(TitleSafeArea.X, TitleSafeArea.Y +
                OurGame.Font.LineSpacing * 2);
            timeTextPosition = new Vector2(TitleSafeArea.X + 2.0f, TitleSafeArea.Y +
                OurGame.Font.LineSpacing * 2 + 2.0f);

            scoreTextShadowPosition = new Vector2(TitleSafeArea.X, TitleSafeArea.Y +
                OurGame.Font.LineSpacing * 3);
            scoreTextPosition = new Vector2(TitleSafeArea.X + 2.0f, TitleSafeArea.Y +
                OurGame.Font.LineSpacing * 3 + 2.0f);

            base.LoadContent();
        }

        private bool CheckCollisions()
        {
            for (int ei = 0; ei < enemyManager.Enemies.Count; ei++)
            {
                //See if an enemy is too close first
                if (enemyManager.Enemies[ei].BoundingSphere.Intersects(playerSphere))
                {
                    GameManager.PushState(OurGame.LostGameState.Value);
                    return (false);
                }

                //if not, then we can check our missiles
                if (missileManager.CheckCollision(enemyManager.Enemies[ei].BoundingSphere))
                {
                    enemyManager.Enemies.RemoveAt(ei);

                    TotalCollisions++;

                    return (true);
                }
            }

            return (false);
        }

        public int CurrentLevel
        {
            get { return (currentLevel); }
        }

        public int Score
        {
            get { return (score); }
        }
    }
}
