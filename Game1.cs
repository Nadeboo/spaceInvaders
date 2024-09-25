using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

//Project overview:
//    Enemies are created through a 2D matrix in enemy.cs
//    Enemies consist of 1,1 sprites with an attached rectangle that's colored in, 
//    to allow for dynamic enemy resizing dependent on how many enemies you want.
//    Enemies will always fill the entire screen width no matter what, and from
//    testing stop being visible at all around 60 or 70 columns. 

//    Player is created in player.cs with a width of 30, height of 30, and position
//    halfway across the screen 10 pixels up from the bottom
//    Whenever you hold left or right the player moves at 5 pixels per frame

//    Projectiles are created in projectile.cs with a width of 15 and height of 10
//    Projectiles move upwards at a speed of 5 pixels per second when created
//    Projectiles are created at the origin point of the player

//    startButton.cs handles clicking the button when you start the game. The mouse
//    colliding with the bounds of the button colors it gray, but otherwise there's
//    nothing interesting going on.

//    Reset.cs contains a ton of methods for resetting the game state

//  game1.cs handles some collision outside of just loading and running the methods
//  from the other classes, that could probably be moved into its own separate class
//  specifically it contains the functions for checking if enemies have hit the bottom
//  of the screen, and for checking if enemies have collided with projectiles,


namespace spaceInvaders
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private StartButton startButton;
        private Reset reset;

        public Player Player;
        public List<Enemy> Enemies;
        public List<Enemy> hardEnemies;
        private List<Enemy> enemiesToDraw;
        public List<Projectile> Projectiles;
        public List<Vector2> InitialEnemyPositions;
        public Texture2D startTexture;
        public Texture2D gameOverTexture;
        public Texture2D tank;
        public Rectangle source;
        int frame;
        double frameTimer, frameInterval;

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

        public enum GameState { Start, InGame, GameOver }
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
            source = new Rectangle(0, 288, 32, 32);



            // Initialization
            startButton = new StartButton(Content, screenWidth, screenHeight);
            Player = Player.Initialize(GraphicsDevice, screenWidth, screenHeight);
            Projectiles = new List<Projectile>();
            (Enemies, hardEnemies, InitialEnemyPositions, EnemyWidth, EnemyHeight) = Enemy.InitializeEnemies(GraphicsDevice, screenWidth, screenHeight);

            // Create a list of enemies to draw (50% of hard enemies)
            Random random = new Random();
            enemiesToDraw = hardEnemies.Where(_ => random.NextDouble() < 0.5).ToList();

            int randomX = random.Next(0, GraphicsDevice.Viewport.Width - source.Width);
            int randomY = random.Next(0, GraphicsDevice.Viewport.Height - source.Height);

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

            //initialize reset code
            reset = new Reset(this); //reset.cs

            InitialMovement = Movement;
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            frameTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (frameTimer <= 0)
            {
                frameTimer = frameInterval;
                frame++;
                if (frame > 10)
                {
                    frame = 0;
                }
                source.X = frame * 32; //Varje delbild är 32 pixlar bred
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
                    // Update player
                    Player.Update(gameTime);

                    // Create and update projectiles
                    if (keyboardState.IsKeyDown(Keys.Space) && gameTime.TotalGameTime - lastShotTime > shootCooldown)
                    {
                        Projectiles.Add(Projectile.Create(GraphicsDevice, Player.GetBounds()));
                        lastShotTime = gameTime.TotalGameTime;
                    }

                    for (int i = Projectiles.Count - 1; i >= 0; i--)
                    {
                        Projectiles[i].Update(gameTime);
                        if (!Projectiles[i].IsActive)
                        {
                            Projectiles.RemoveAt(i);
                        }
                    }

                    // Check if enemies have collided with the bottom of the screen
                    bool anyEnemyHitBottom = Enemies.Any(enemy => enemy.GetBounds().Intersects(bottomBoundary));
                    if (anyEnemyHitBottom)
                    {
                        reset.ResetEnemies();
                        reset.DecrementLives();
                        if (Lives <= 0)
                        {
                            CurrentGameState = GameState.GameOver;
                        }
                    }

                    // Check collisions between projectiles and enemies

                    // Collision detection for regular enemies
                    for (int i = Projectiles.Count - 1; i >= 0; i--)
                    {
                        bool collided = false;
                        for (int j = Enemies.Count - 1; j >= 0; j--)
                        {
                            if (Projectiles[i].GetBounds().Intersects(Enemies[j].GetBounds()))
                            {
                                Projectiles.RemoveAt(i);
                                Enemies.RemoveAt(j);
                                Score++;
                                collided = true;
                                break;
                            }
                        }
                        if (collided) continue;
                    }

                    // Collision detection for hard enemies
                    for (int k = Projectiles.Count - 1; k >= 0; k--)
                    {
                        bool hardCollided = false;
                        for (int l = hardEnemies.Count - 1; l >= 0; l--)
                        {
                            if (Projectiles[k].GetBounds().Intersects(hardEnemies[l].GetBounds()))
                            {
                                Projectiles.RemoveAt(k);
                                hardEnemies.RemoveAt(l);
                                Score++;
                                hardCollided = true;
                                break;
                            }
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
            }
            // Update player
            Player.Update(gameTime);

            // Create and update projectiles
            if (keyboardState.IsKeyDown(Keys.Space) && gameTime.TotalGameTime - lastShotTime > shootCooldown)
            {
                Projectiles.Add(Projectile.Create(GraphicsDevice, Player.GetBounds()));
                lastShotTime = gameTime.TotalGameTime;
            }

            for (int i = Projectiles.Count - 1; i >= 0; i--)
            {
                Projectiles[i].Update(gameTime);
                if (!Projectiles[i].IsActive)
                {
                    Projectiles.RemoveAt(i);
                }
            }

            // Update enemy movement
            Enemy.UpdateMovement(Enemies, ref Movement, gameTime);
            Enemy.UpdateMovement(hardEnemies, ref Movement, gameTime);

            if (keyboardState.IsKeyDown(Keys.R))
            {
                reset.ResetGame();
            }

            CheckBottomBoundaryCollision();

            base.Update(gameTime);
        }


        private void CheckBottomBoundaryCollision()
        {
            bool anyEnemyHitBottom = false;
            foreach (var enemy in Enemies)
            {
                if (enemy.GetBounds().Intersects(bottomBoundary))
                {
                    anyEnemyHitBottom = true;
                    break;
                }
            }

            if (anyEnemyHitBottom)
            {
                reset.ResetEnemies();
                reset.DecrementLives();
            }
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

                    // Draw enemies
                    foreach (var enemy in Enemies)
                    {
                        enemy.Draw(spriteBatch);
                    }

                    foreach (var hardEnemy in hardEnemies)
                    {
                        hardEnemy.DrawWhite(spriteBatch);
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
                    spriteBatch.Draw(gameOverTexture, Vector2.Zero, Color.White);

                    spriteBatch.Draw(tank, new Vector2(randomX1, randomY1), source, Color.White);
                    spriteBatch.Draw(tank, new Vector2(randomX2, randomY2), source, Color.White);
                    spriteBatch.Draw(tank, new Vector2(randomX3, randomY3), source, Color.White);
                    spriteBatch.Draw(tank, new Vector2(randomX4, randomY4), source, Color.White);
                    spriteBatch.Draw(tank, new Vector2(randomX5, randomY5), source, Color.White);

                    string gameOverText = "Game Over!";
                    string restartText = "Press R to restart";
                    Vector2 gameOverSize = scoreFont.MeasureString(gameOverText);
                    Vector2 restartSize = scoreFont.MeasureString(restartText);

                    spriteBatch.DrawString(scoreFont, gameOverText,
                        new Vector2((GraphicsDevice.Viewport.Width - gameOverSize.X) / 2,
                        GraphicsDevice.Viewport.Height / 2 - gameOverSize.Y),
                        Color.Red);

                    spriteBatch.DrawString(scoreFont, restartText,
                        new Vector2((GraphicsDevice.Viewport.Width - restartSize.X) / 2,
                        GraphicsDevice.Viewport.Height / 2 + restartSize.Y),
                        Color.White);
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

        //loops through all projectiles, and then for every projectile,
        //loops through all enemies to check if they've collided
        //if they have, enemy and projectile are destroyed
