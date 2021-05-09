using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THClone
{
    class GameOverState : State
    {
        private ContentManager manager;

        private Viewport viewport;

        private Texture2D backTex;

        private SpriteFont font;

        private string text;

        public bool BackToMenu { get; private set; }

        public void Initialise(Viewport viewport)
        {
            manager = new ContentManager(GameInfo.GameInstance.Services, GameInfo.GameInstance.Content.RootDirectory);
            this.viewport = viewport;
        }

        public void LoadContent()
        {
            backTex = manager.Load<Texture2D>("Graphics\\black_background");
            font = manager.Load<SpriteFont>("Graphics\\gameFont");
        }

        public void UnloadContent()
        {
            manager.Unload();
        }

        public override void Enter(object owner, State prevState = null)
        {
            BackToMenu = false;
            LoadContent();

            if (GameInfo.BossDestroyed)
                text = "Congratulations!";
            else
                text = "Game over!";

            CommandManager.Instance.AddKeyboardBinding(Keys.Q, (bState, _) => BackToMenu = bState == eButtonState.PRESSED);
        }

        public override void Execute(object owner, GameTime gameTime)
        {
        }

        public override void Exit(object owner, State nextState = null)
        {
            UnloadContent();

            CommandManager.Instance.RemoveKeyboardBinding(Keys.Q);
        }

        public override void Draw(object owner, SpriteBatch spriteBatch)
        {
            var rect = new Rectangle(0, 0, viewport.Width, viewport.Height);
            spriteBatch.Draw(backTex, rect, Color.White);
            spriteBatch.DrawString(font, text, new Vector2(viewport.TitleSafeArea.Width / 2f, (viewport.TitleSafeArea.Height / 2f)), Color.White);
            spriteBatch.DrawString(font, "Your score is: " + GameInfo.CurrentScore, new Vector2(viewport.TitleSafeArea.Width / 2f, (viewport.TitleSafeArea.Height / 2f + 40)), Color.White);
            spriteBatch.DrawString(font, "Press Q to go back to main menu", new Vector2(viewport.TitleSafeArea.Width / 2f, (viewport.TitleSafeArea.Height / 2f + 80f)), Color.White);
        }
    }
}
