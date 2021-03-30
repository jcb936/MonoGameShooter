using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace THClone
{
    class HUD
    {
        private Texture2D textureHUD;

        private Rectangle positionHUD;

        private SpriteFont font;

        private Viewport viewport;

        public void Initialize(Viewport viewport, Texture2D texture, SpriteFont font)
        {
            this.viewport = viewport;
            this.font = font;
            textureHUD = texture;
            positionHUD = new Rectangle(0, 0, viewport.Width, (int)(viewport.Height / 10f));
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureHUD, positionHUD, new Color(255, 255, 255, 125));

            // fix where to draw hud
            // Draw the score
            spriteBatch.DrawString(font, "score: " + GameInfo.CurrentScore, new Vector2(viewport.TitleSafeArea.X, viewport.TitleSafeArea.Y), Color.White);
            // Draw the player health
            spriteBatch.DrawString(font, "health: " + GameInfo.Health, new Vector2(viewport.TitleSafeArea.X, viewport.TitleSafeArea.Y + 30), Color.White);
        }
    }
}
