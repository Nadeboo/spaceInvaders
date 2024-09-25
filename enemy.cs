using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace spaceInvaders
{
    public class Enemy
    {
        private Texture2D pixel;
        private double xPosition;
        private double yPosition;
        private int width;
        private int height;
        private double speed = 2; //enemy speed
        private Vector2 initialPosition;

        public static (List<Enemy> regularEnemies, List<Enemy> hardEnemies, List<Vector2> initialPositions, int enemyWidth, int enemyHeight)
        InitializeEnemies(GraphicsDevice graphicsDevice, int screenWidth, int screenHeight)
        {
            List<Enemy> regularEnemies = new List<Enemy>();
            List<Enemy> hardEnemies = new List<Enemy>();
            List<Vector2> initialPositions = new List<Vector2>();

            int enemyNum = 10; // num of columns
            int numRows = 10; //num of rows. multiply together for the total number of enemies
            int totalSpaceWidth = (enemyNum - 1) * 10;
            int totalRectWidth = screenWidth - totalSpaceWidth;
            int enemyWidth = totalRectWidth / enemyNum;
            int enemyHeight = screenHeight / 20;
            int hardEnemyWidth = enemyWidth ;
            int hardEnemyHeight = enemyHeight;
            int yOffset = 50;
            int xOffset = 0;

            for (int j = 0; j < numRows; j++)
            {
                int yPosition = j * (enemyHeight + 10) + yOffset;

                for (int i = 0; i < enemyNum; i++)
                {
                    int xPosition = i * (enemyWidth + 10) + xOffset;
                    Vector2 position = new Vector2(xPosition, yPosition);

                    initialPositions.Add(position);
                    regularEnemies.Add(new Enemy(graphicsDevice, xPosition, yPosition, enemyWidth, enemyHeight));

                    // 50% chance to add a hard enemy at the same position
                    if (new Random().Next(2) == 0)
                    {
                        hardEnemies.Add(new Enemy(graphicsDevice, xPosition, yPosition, hardEnemyWidth, hardEnemyHeight));
                    }
                }
            }

            return (regularEnemies, hardEnemies, initialPositions, enemyWidth, enemyHeight);
        }


        public Rectangle GetBounds()
        {
            return new Rectangle((int)xPosition, (int)yPosition, width, height);
        }
        public void SetInitialPosition(Vector2 position)
        {
            initialPosition = position;
        }
        public Vector2 GetPosition()
        {
            return new Vector2((float)xPosition, (float)yPosition);
        }
        public void ResetToInitialPosition()
        {
            xPosition = initialPosition.X;
            yPosition = initialPosition.Y;
        }

        //initializes a rectangle with width and height
        public Enemy(GraphicsDevice graphicsDevice, int x, int y, int width, int height)
        {
            this.xPosition = x;
            this.yPosition = y;
            this.width = width;
            this.height = height;

            //initializes a 1,1 white pixel sprite
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        public void downMovement(GameTime gameTime)
        {
            yPosition += speed;
        }

        public void leftMovement(GameTime gameTime)
        {
            xPosition -= speed;
        }

        public void rightMovement(GameTime gameTime)
        {
            xPosition += speed;
        }

        public static void UpdateMovement(List<Enemy> enemies, ref double movement, GameTime gameTime)
        {
            movement += 0.1;

            switch (true)
            {
                case bool when movement >= 0 && movement < 30:
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.leftMovement(gameTime);
                    }
                    break;
                case bool when movement >= 30 && movement < 60:
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.rightMovement(gameTime);
                    }
                    break;
                case bool when movement >= 60 && movement < 90:
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.downMovement(gameTime);
                    }
                    break;
                case bool when movement >= 90 && movement < 120:
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.rightMovement(gameTime);
                    }
                    break;
                case bool when movement >= 120 && movement < 150:
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.leftMovement(gameTime);
                    }
                    break;
                case bool when movement >= 150:
                    movement = 0;
                    break;
            }
        }



        public void Draw(SpriteBatch spriteBatch)
        {
            //draws the pixel sprite from earlier using position, width and height
            spriteBatch.Draw(pixel, new Rectangle((int)xPosition, (int)yPosition, width, height), Color.Red);
        }
        public void DrawWhite(SpriteBatch spriteBatch)
        {
            //draws the pixel sprite from earlier using position, width and height
            spriteBatch.Draw(pixel, new Rectangle((int)xPosition, (int)yPosition, width, height), Color.White);
        }
    }
}