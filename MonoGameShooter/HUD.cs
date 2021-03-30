using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameShooter
{
    class HUD
    {
        private Texture2D textureHUD;

        private Rectangle positionHUD;

        public void Initialize(int screenHeight, int screenWidth, Texture2D texture)
        {
            textureHUD = texture;
            positionHUD = new Rectangle(0, 0, screenWidth, (int)(screenHeight / 10f));
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureHUD, positionHUD, new Color(255, 255, 255, 125));
        }
    }
}
