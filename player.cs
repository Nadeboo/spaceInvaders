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
    internal class Player
    {

        public Vector2 GetPosition()
        {
            return new Vector2(playerX, playerY);
        }
        public Rectangle GetBounds()
        {
            return new Rectangle(playerX, playerY, playerWidth, playerHeight);
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
