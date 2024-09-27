using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace spaceInvaders
{
    public class Player
    {
        public static Player Initialize(GraphicsDevice graphicsDevice, int screenWidth, int screenHeight)
        {
            int playerWidth = 30;
            int playerHeight = 30;
            int playerX = (screenWidth - playerWidth) / 2;
            int playerY = screenHeight - playerHeight - 10;

            Player player = new Player(graphicsDevice, playerX, playerY, playerWidth, playerHeight);
            player.SetInitialPosition(new Vector2(playerX, playerY));

            return player;
        }

        private Vector2 initialPosition;

        public Vector2 GetPosition()
        {
            return new Vector2(playerX, playerY);
        }
        public Rectangle GetBounds()
        {
            return new Rectangle(playerX, playerY, playerWidth, playerHeight);
        }
        public void SetInitialPosition(Vector2 position)
        {
            initialPosition = position;
        }

        public void ResetPosition()
        {
            playerX = (int)initialPosition.X;
            playerY = (int)initialPosition.Y;
        }

        private Texture2D pixel;
        public int playerX;
        public int playerY;
        public int playerWidth;
        public int playerHeight;
        public Player(GraphicsDevice graphicsDevice, int x, int y, int width, int height)
        {
            this.playerX = x;
            this.playerY = y;
            this.playerWidth = width;
            this.playerHeight = height;

            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }
        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                playerX -= 5; 
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                playerX += 5;
            }

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(pixel, new Rectangle(playerX, playerY, playerWidth, playerHeight), Color.Red);
        }
    }
}
