using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace spaceInvaders
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private StartButton startButton;
        private Reset reset;

        public Player Player;
        public Enemy[,] Enemies; 
        public Enemy[,] hardEnemies; 
        public Vector2[,] InitialEnemyPositions;
        private List<Enemy> enemiesToDraw;
        public List<Projectile> Projectiles;
        public Texture2D startTexture;
        public Texture2D gameOverTexture;
        public Texture2D tank;
        public Rectangle source;
        int frame;
        double frameTimer, frameInterval;

        private Texture2D youwin;
        private Texture2D youlose;

        private SpriteFont scoreFont;
        private Rectangle bottomBoundary;

        public int Score;
        public int Lives;
        private int startX;
        private int startY;
        public int EnemyWidth;
        public int EnemyHeight;

        private int randomX1, randomY1, randomX2, randomY2, randomX3, randomY3, randomX4, randomY4, randomX5, randomY5;

        public double Movement;
        public double InitialMovement;

        private TimeSpan shootCooldown = TimeSpan.FromMilliseconds(1000);
        private TimeSpan lastShotTime = TimeSpan.Zero;

        public enum GameState { Start, InGame, GameOver, GameWon }
        public GameState CurrentGameState;
        private Random random = new Random();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Lives = 1;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
        }

        //creates a rectangle across the bottom of the screen
        //sets gamestate to start
        protected override void Initialize()
        {
            bottomBoundary = new Rectangle(0, GraphicsDevice.Viewport.Height - 10, GraphicsDevice.Viewport.Width, 10);
            CurrentGameState = GameState.Start;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            scoreFont = Content.Load<SpriteFont>("score");
            tank = Content.Load<Texture2D>("SpriteSheet_Tanks");


            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;
            startTexture = Content.Load<Texture2D>("bender");
            gameOverTexture = Content.Load<Texture2D>("ride");
            youwin = Content.Load<Texture2D>("youwin");
            youlose = Content.Load<Texture2D>("youlose");
            source = new Rectangle(0, 288, 32, 32);



            // Initialization
            startButton = new StartButton(Content, screenWidth, screenHeight);
            Player = Player.Initialize(GraphicsDevice, screenWidth, screenHeight);
            Projectiles = new List<Projectile>();
            (Enemies, hardEnemies, InitialEnemyPositions, EnemyWidth, EnemyHeight) = Enemy.InitializeEnemies(GraphicsDevice, screenWidth, screenHeight);

            int randomX = random.Next(0, GraphicsDevice.Viewport.Width - source.Width);
            int randomY = random.Next(0, GraphicsDevice.Viewport.Height - source.Height);

            //for drawing the 5 random sprites on the end screen
            //i didn't feel like giving it any effort
            randomX1 = random.Next(0, 1920);
            randomY1 = random.Next(0, 1080);

            randomX2 = random.Next(0, 1920);
            randomY2 = random.Next(0, 1080);

            randomX3 = random.Next(0, 1920);
            randomY3 = random.Next(0, 1080);

            randomX4 = random.Next(0, 1920);
            randomY4 = random.Next(0, 1080);

            randomX5 = random.Next(0, 1920);
            randomY5 = random.Next(0, 1080);

            reset = new Reset(this); //reset.cs

            InitialMovement = Movement;
        }


        private bool AreAllEnemiesDestroyed()
        {
            for (int i = 0; i < Enemies.GetLength(0); i++)
            {
                for (int j = 0; j < Enemies.GetLength(1); j++)
                {
                    if (Enemies[i, j] != null)
                    {
                        return false;
                    }
                }
            }

            for (int i = 0; i < hardEnemies.GetLength(0); i++)
            {
                for (int j = 0; j < hardEnemies.GetLength(1); j++)
                {
                    if (hardEnemies[i, j] != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            //i think this is exclusively used for the animated tank sprites
            frameTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (frameTimer <= 0)
            {
                frameTimer = frameInterval;
                frame++;
                if (frame > 10)
                {
                    frame = 0;
                }
                source.X = frame * 32;
            }

            switch (CurrentGameState)
            {
                case GameState.Start:
                    startButton.Update(mouseState);
                    if (startButton.IsClicked())
                    {
                        CurrentGameState = GameState.InGame;
                    }
                    break;

                case GameState.InGame:
                    Player.Update(gameTime);

                    // Create and update projectiles
                    //if total game time minus time of the last shot is larger than shootcoldown:
                    //a full second has passed and we can shoot again
                    if (keyboardState.IsKeyDown(Keys.Space) && gameTime.TotalGameTime - lastShotTime > shootCooldown)
                    {
                        Projectiles.Add(Projectile.Create(GraphicsDevice, Player.GetBounds()));
                        lastShotTime = gameTime.TotalGameTime;
                    }

                    //checks if projectiles have moved above the screen
                    for (int i = Projectiles.Count - 1; i >= 0; i--)
                    {
                        Projectiles[i].Update(gameTime);
                        if (!Projectiles[i].IsActive)
                        {
                            Projectiles.RemoveAt(i);
                        }
                    }

                    // Check if enemies have collided with the bottom of the screen
                    for (int i = 0; i < Enemies.GetLength(0); i++)
                    {
                        for (int j = 0; j < Enemies.GetLength(1); j++)
                        {
                            if (Enemies[i, j] != null && Enemies[i, j].GetBounds().Intersects(bottomBoundary))
                            {
                                reset.ResetEnemies();
                                reset.DecrementLives();
                                if (Lives <= 0)
                                {
                                    CurrentGameState = GameState.GameOver;
                                }
                                return;
                            }
                        }
                    }
                    if (AreAllEnemiesDestroyed())
                    {
                        CurrentGameState = GameState.GameWon;
                    }

                    if (Enemies.GetLength(0) == 0 && hardEnemies.GetLength(0) == 0)
                    {
                        CurrentGameState = GameState.GameWon;
                    }

                    // Check collisions between projectiles and enemies

                    // Collision detection for regular enemies
                    //checks all projectiles, and then iterates through the enemy array to check if they intersect
                    for (int i = Projectiles.Count - 1; i >= 0; i--)
                    {
                        bool collided = false;
                        for (int j = Enemies.GetLength(1) - 1; j >= 0; j--)
                        {
                            for (int k = Enemies.GetLength(0) - 1; k >= 0; k--)
                            {
                                if (Enemies[k, j] != null && Projectiles[i].GetBounds().Intersects(Enemies[k, j].GetBounds()))
                                {
                                    Projectiles.RemoveAt(i);
                                    Enemies[k, j] = null;
                                    Score++;
                                    collided = true;
                                    break;
                                }
                            }
                            if (collided) break;
                        }
                        if (collided) continue;
                    }

                    // Collision detection for hard enemies
                    //same as above
                    for (int k = Projectiles.Count - 1; k >= 0; k--)
                    {
                        bool hardCollided = false;
                        for (int l = hardEnemies.GetLength(1) - 1; l >= 0; l--)
                        {
                            for (int m = hardEnemies.GetLength(0) - 1; m >= 0; m--)
                            {
                                if (hardEnemies[m, l] != null && Projectiles[k].GetBounds().Intersects(hardEnemies[m, l].GetBounds()))
                                {
                                    Projectiles.RemoveAt(k);
                                    hardEnemies[m, l] = null; 
                                    Score += 2; 
                                    hardCollided = true;
                                    break;
                                }
                            }
                            if (hardCollided) break;
                        }
                        if (hardCollided) continue;
                    }
                    break;

                case GameState.GameOver:
                    if (keyboardState.IsKeyDown(Keys.R))
                    {
                        reset.ResetGame();
                        CurrentGameState = GameState.InGame;
                    }
                    break;
                case GameState.GameWon:
                    if (keyboardState.IsKeyDown(Keys.R))
                    {
                        reset.ResetGame();
                        CurrentGameState = GameState.InGame;
                    }
                    break;
            }
            // Update player
            Player.Update(gameTime);

            // Create and update projectiles
            if (keyboardState.IsKeyDown(Keys.Space) && gameTime.TotalGameTime - lastShotTime > shootCooldown)
            {
                Projectiles.Add(Projectile.Create(GraphicsDevice, Player.GetBounds()));
                lastShotTime = gameTime.TotalGameTime;
            }

            Enemy.UpdateMovement(Enemies, ref Movement, gameTime);
            Enemy.UpdateMovement(hardEnemies, ref Movement, gameTime);

            if (keyboardState.IsKeyDown(Keys.R))
            {
                reset.ResetGame();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            switch (CurrentGameState)
            {
                case GameState.Start:
                    spriteBatch.Draw(startTexture, Vector2.Zero, Color.White);
                    spriteBatch.Draw(tank, new Vector2(100, 100), source, Color.White);
                    startButton.Draw(spriteBatch);
                    break;

                case GameState.InGame:
                    // Draw the bottom boundary rectangle
                    Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
                    pixel.SetData(new[] { Color.White });
                    spriteBatch.Draw(pixel, bottomBoundary, Color.White);

                    // Draw enemy if it isn't null
                    for (int i = 0; i < Enemies.GetLength(0); i++)
                    {
                        for (int j = 0; j < Enemies.GetLength(1); j++)
                        {
                            if (Enemies[i, j] != null)
                            {
                                Enemies[i, j].Draw(spriteBatch);
                            }
                        }
                    }

                    //like above, but for white enemies
                    for (int i = 0; i < hardEnemies.GetLength(0); i++)
                    {
                        for (int j = 0; j < hardEnemies.GetLength(1); j++)
                        {
                            if (hardEnemies[i, j] != null)
                            {
                                hardEnemies[i, j].DrawWhite(spriteBatch);
                            }
                        }
                    }




                    // Draw projectiles
                    foreach (var projectile in Projectiles)
                    {
                        projectile.Draw(spriteBatch);
                    }

                    // Draw player
                    Player.Draw(spriteBatch);

                    // Draw score
                    spriteBatch.DrawString(scoreFont, "Score: " + Score,
                        new Vector2(10, 10), Color.White, 0, Vector2.Zero,
                        1, SpriteEffects.None, 1);

                    // Draw lives
                    spriteBatch.DrawString(scoreFont, "Lives: " + Lives,
                        new Vector2(GraphicsDevice.Viewport.Width - 100, 10), Color.White);
                    break;

                case GameState.GameOver:
                    spriteBatch.Draw(youlose, Vector2.Zero, Color.White);

                    //sprites on end screen
                    spriteBatch.Draw(tank, new Vector2(randomX1, randomY1), source, Color.White);
                    spriteBatch.Draw(tank, new Vector2(randomX2, randomY2), source, Color.White);
                    spriteBatch.Draw(tank, new Vector2(randomX3, randomY3), source, Color.White);
                    spriteBatch.Draw(tank, new Vector2(randomX4, randomY4), source, Color.White);
                    spriteBatch.Draw(tank, new Vector2(randomX5, randomY5), source, Color.White);

                    break;

                case GameState.GameWon:
                    spriteBatch.Draw(youwin, Vector2.Zero, Color.White);
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
