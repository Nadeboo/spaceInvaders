using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace spaceInvaders
{
    public class Enemy
    {
        private Texture2D pixel;
        private double xPosition;
        private double yPosition;
        private int width;
        private int height;
        public double speed;

        public static (Enemy[,] regularEnemies, Enemy[,] hardEnemies, Vector2[,] initialPositions, int enemyWidth, int enemyHeight)
        InitializeEnemies(GraphicsDevice graphicsDevice, int screenWidth, int screenHeight, int levelNumber)
        {

            int enemyNum = 10; // num of columns
            int numRows = 1; // num of rows. multiply together for the total number of enemies
            int totalSpaceWidth = (enemyNum - 1) * 10;
            int totalRectWidth = screenWidth - totalSpaceWidth;
            int enemyWidth = totalRectWidth / enemyNum;
            int enemyHeight = screenHeight / 20;
            int hardEnemyWidth = enemyWidth;
            int hardEnemyHeight = enemyHeight;
            int yOffset = 50;
            int xOffset = 0;
            double speed = 0.5 + (levelNumber * 100);

            Enemy[,] regularEnemiesArray = new Enemy[numRows, enemyNum];
            Enemy[,] hardEnemiesArray = new Enemy[numRows, enemyNum];
            Vector2[,] initialPositionsArray = new Vector2[numRows, enemyNum];

            Random random = new Random();

            //for every row:
            //for number of enemies:
            for (int j = 0; j < numRows; j++)
            {
                int yPosition = j * (enemyHeight + 10) + yOffset;

                for (int i = 0; i < enemyNum; i++)
                {
                    //each enemy will be placed right after the other (with a 10 pixel gap)
                    //on both the y axis and x axis
                    int xPosition = i * (enemyWidth + 10) + xOffset;
                    Vector2 position = new Vector2(xPosition, yPosition);

                    //saves the original positions of enemies
                    initialPositionsArray[j, i] = position;

                    //creates a new instance of an enemy for each of the previously created positions
                    regularEnemiesArray[j, i] = new Enemy(graphicsDevice, xPosition, yPosition, enemyWidth, enemyHeight, speed);

                    // 50% chance to add a hard enemy at the same position
                    if (random.Next(2) == 0)
                    {
                        hardEnemiesArray[j, i] = new Enemy(graphicsDevice, xPosition, yPosition, hardEnemyWidth, hardEnemyHeight, speed);
                    }
                }
            }

            return (regularEnemiesArray, hardEnemiesArray, initialPositionsArray, enemyWidth, enemyHeight);
        }

        //creates a new rectangle for each previously created enemy
        public Rectangle GetBounds()
        {
            return new Rectangle((int)xPosition, (int)yPosition, width, height);
        }

        // initializes enemy
        public Enemy(GraphicsDevice graphicsDevice, int x, int y, int width, int height, double speed)
        {
            this.xPosition = x;
            this.yPosition = y;
            this.width = width;
            this.height = height;
            this.speed = speed;

            // initializes a 1,1 white pixel sprite
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

        public static void UpdateMovement(Enemy[,] enemies, ref double movement, GameTime gameTime)
        {
            movement += 0.1;

            switch (true)
            {
                case bool when movement >= 0 && movement < 30:
                    foreach (Enemy enemy in enemies)
                    {
                        if (enemy != null) enemy.leftMovement(gameTime);
                    }
                    break;
                case bool when movement >= 30 && movement < 60:
                    foreach (Enemy enemy in enemies)
                    {
                        if (enemy != null) enemy.rightMovement(gameTime);
                    }
                    break;
                case bool when movement >= 60 && movement < 90:
                    foreach (Enemy enemy in enemies)
                    {
                        if (enemy != null) enemy.downMovement(gameTime);
                    }
                    break;
                case bool when movement >= 90 && movement < 120:
                    foreach (Enemy enemy in enemies)
                    {
                        if (enemy != null) enemy.rightMovement(gameTime);
                    }
                    break;
                case bool when movement >= 120 && movement < 150:
                    foreach (Enemy enemy in enemies)
                    {
                        if (enemy != null) enemy.leftMovement(gameTime);
                    }
                    break;
                case bool when movement >= 150:
                    movement = 0;
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // draws the pixel sprite from earlier using position, width and height
            spriteBatch.Draw(pixel, new Rectangle((int)xPosition, (int)yPosition, width, height), Color.Red);
        }

        public void DrawWhite(SpriteBatch spriteBatch)
        {
            // draws the pixel sprite from earlier using position, width and height
            spriteBatch.Draw(pixel, new Rectangle((int)xPosition, (int)yPosition, width, height), Color.White);
        }
    }
}