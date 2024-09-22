using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

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
                int yPosition = j * (rectHeight + 10);
                for (int i = 0; i < enemyNum; i++)
                {
                    int xPosition = i * (rectWidth + 10);
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
                Projectile newProjectile = new Projectile(GraphicsDevice, projectileX, projectileY, 5, 10);

                //add a new projectile to projectiles list
                projectiles.Add(newProjectile);

                lastShotTime = gameTime.TotalGameTime;
            }
            //for every projectile in projectiles: update projectiles
            foreach (var projectile in projectiles)
            {
                projectile.Update(gameTime);
            }

            CheckCollisions();
            player.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

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

            base.Draw(gameTime);
        }
    }
}