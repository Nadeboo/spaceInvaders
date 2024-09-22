using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace spaceInvaders
{
    internal class Enemy
    {
        private Texture2D pixel;
        private int xPosition;
        private int yPosition;
        private int width;
        private int height;

        public Rectangle GetBounds()
        {
            return new Rectangle(xPosition, yPosition, width, height);
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

        public void Draw(SpriteBatch spriteBatch)
        {
            //draws the pixel sprite from earlier using position, width and height
            //y-value goes unused
            spriteBatch.Draw(pixel, new Rectangle(xPosition, yPosition, width, height), Color.Red);
        }
    }
}