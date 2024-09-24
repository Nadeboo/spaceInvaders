using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace spaceInvaders
{
    public class Projectile
    {
        private Texture2D pixel;
        private Vector2 position;
        private int width;
        private int height;
        private int speed = 5;

        public bool IsActive { get; private set; } = true;
        public Rectangle GetBounds()
        {
            return new Rectangle((int)position.X, (int)position.Y, width, height);
        }

        public Projectile(GraphicsDevice graphicsDevice, int x, int y, int width, int height)
        {
            this.position = new Vector2(x, y);
            this.width = width;
            this.height = height;
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        public static Projectile Create(GraphicsDevice graphicsDevice, Rectangle playerBounds)
        {
            int projectileWidth = 15;
            int projectileHeight = 10;
            int projectileX = playerBounds.Center.X - (projectileWidth / 2);
            int projectileY = playerBounds.Top - projectileHeight;

            return new Projectile(graphicsDevice, projectileX, projectileY, projectileWidth, projectileHeight);
        }

        public void Update(GameTime gameTime)
        {
            position.Y -= speed;
            if (position.Y + height < 0)
            {
                IsActive = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(pixel, new Rectangle((int)position.X, (int)position.Y, width, height), Color.White);
        }
    }
}