using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;

namespace spaceInvaders
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
        private List<Enemy> enemies;
        private Player player;
        private List<Projectile> projectiles;
        private TimeSpan shootCooldown = TimeSpan.FromMilliseconds(1000);
        private TimeSpan lastShotTime = TimeSpan.Zero;
        private int score;
        SpriteFont scoreFont;
        int startX;
        int startY;
        private double movement = 0;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //enemy.cs, projectile.cs
            spriteBatch = new SpriteBatch(GraphicsDevice);
            enemies = new List<Enemy>();
            projectiles = new List<Projectile>();

            int enemyNum = 20;
            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;
            int totalSpaceWidth = (enemyNum - 1) * 10;
            int totalRectWidth = screenWidth - totalSpaceWidth;
            int rectWidth = totalRectWidth / enemyNum;
            int rectHeight = screenHeight / 20;
            int numRows = 4;
            int movement = 0;

            int score = 0;
            int yOffset = 50;
            int xOffset = 0;
            scoreFont = Content.Load<SpriteFont>("score");

            // Player setup
            int playerWidth = rectWidth;
            int playerHeight = rectHeight;
            int playerX = (screenWidth - playerWidth) / 2;
            int playerY = screenHeight - playerHeight - 10;

            //player.cs
            player = new Player(GraphicsDevice, playerX, playerY, playerWidth, playerHeight);

            // Enemy setup
            //create a 2D matrix of enemies
            //the width of individual enemies will always adjust
            //to fit the total width of the screen
            for (int j = 0; j < numRows; j++)
            {
                int yPosition = j * (rectHeight + 10) + yOffset;
                if (j == 0)
                {
                    startY = yPosition;
                }
                for (int i = 0; i < enemyNum; i++)
                {
                    int xPosition = i * (rectWidth + 10) + xOffset;
                    if (j == 0 && i == 0)
                    {
                        startX = xPosition;
                    }
                    enemies.Add(new Enemy(GraphicsDevice, xPosition, yPosition, rectWidth, rectHeight));
                }
            }
        }
        private void CheckCollisions()
        {
            //iterates backwards through all projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                //for each projectile: iterates backwards
                //through all enemies
                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    //each projectile, each frame, checks if it intersects
                    //with any enemy (checking all of them)
                    //and if it does:
                    if (projectiles[i].GetBounds().Intersects(enemies[j].GetBounds()))
                    {
                        //it removes the projectile and the enemy
                        projectiles.RemoveAt(i);
                        enemies.RemoveAt(j);
                        score++;
                        break;
                    }
                }
            }
        }
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            //if space is held down and a second or more
            //has passed since the last shot:
            //create a new projectile with origins at the player
            if (keyboardState.IsKeyDown(Keys.Space) && gameTime.TotalGameTime - lastShotTime > shootCooldown)
            {
                Rectangle playerBounds = player.GetBounds();
                int projectileX = playerBounds.Center.X;
                int projectileY = playerBounds.Top - 10;
                Projectile newProjectile = new Projectile(GraphicsDevice, projectileX, projectileY, 15, 10);

                //add a new projectile to projectiles list
                projectiles.Add(newProjectile);

                lastShotTime = gameTime.TotalGameTime;
            }
            //for every projectile in projectiles: update projectiles
            foreach (var projectile in projectiles)
            {
                projectile.Update(gameTime);
            }
            movement += 0.1;

            //goes through a loop consisting of 120 steps:
            //changes movement type every 30 steps
            //goes right -> down -> left -> down -> repeat
            movement += 0.1;

            switch (true)
            {
                case bool when movement >= 0 && movement < 30:
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.rightMovement(gameTime);
                    }
                    break;

                case bool when movement >= 30 && movement < 60:
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.downMovement(gameTime);
                    }
                    break;

                case bool when movement >= 60 && movement < 90:
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.leftMovement(gameTime);
                    }
                    break;

                case bool when movement >= 90:
                    movement = 0;
                    break;
            }

            CheckCollisions();
            player.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            //for every enemy in enemies: draw an enemy
            foreach (var enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }
            //for every projectile in projectiles: draw a projectile
            foreach (var projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }

            //draw player to the screen
            player.Draw(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.DrawString(scoreFont, "Score: " + score,
                new Vector2(10, 10), Color.White, 0, Vector2.Zero,
                1, SpriteEffects.None, 1);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}