using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace spaceInvaders
{
    public class StartButton
    {
        private Rectangle bounds;
        private Texture2D texture;
        private Color color;
        private bool isClicked;

        public StartButton(ContentManager content, int screenWidth, int screenHeight)
        {
            texture = content.Load<Texture2D>("startKnapp");

            // Calculate position to center the button
            int x = (screenWidth - texture.Width) / 2;
            int y = (screenHeight - texture.Height) / 2;

            // Set the bounds based on the texture size
            bounds = new Rectangle(x, y, texture.Width, texture.Height);
            color = Color.White;
            isClicked = false;
        }

        public void Update(MouseState mouseState)
        {
            if (bounds.Contains(mouseState.Position))
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    color = Color.Gray;
                    isClicked = true;
                }
                else
                {
                    color = Color.LightGray;
                    isClicked = false;
                }
            }
            else
            {
                color = Color.White;
                isClicked = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, bounds, color);
        }

        public bool IsClicked()
        {
            return isClicked;
        }
    }
}