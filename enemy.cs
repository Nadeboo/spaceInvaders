using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace spaceInvaders
{
    internal class Enemy
    {
        private Texture2D pixel;
        private double xPosition;
        private double yPosition;
        private int width;
        private int height;
        private double speed = 0.1; // Reduced speed for smoother movement

        public Rectangle GetBounds()
        {
            return new Rectangle((int)xPosition, (int)yPosition, width, height);
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

        public void Draw(SpriteBatch spriteBatch)
        {
            //draws the pixel sprite from earlier using position, width and height
            spriteBatch.Draw(pixel, new Rectangle((int)xPosition, (int)yPosition, width, height), Color.Red);
        }
    }
}