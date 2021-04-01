using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace THClone
{
    class MenuState : State
    {
        public bool StartGame { get; private set; }

        private Texture2D backgroundImage;

        private SpriteFont font;

        private enum Selection
        {
            START = 0,
            HIGH_SCORE = 1,
            QUIT = 2
        }

        private enum Screen
        {
            MAIN,
            HIGH_SCORE
        }

        private Screen currentScreen;

        private Selection currentSelection;

        private float currentPointerYLocation;

        private Action exitGame;

        private Viewport viewport;

        public void Initialize(Texture2D backgroundImage, SpriteFont font, Viewport viewport)
        {
            Name = "MenuState";
            this.backgroundImage = backgroundImage;
            this.font = font;
            this.viewport = viewport;
        }

        public override void Enter(object owner)
        {
            currentSelection = Selection.START;
            currentScreen = Screen.MAIN;
            currentPointerYLocation = viewport.TitleSafeArea.Width / 2f;
            StartGame = false;
            if (owner.GetType() == typeof(Game))
                exitGame = () => (owner as Game).Exit();

            CommandManager.Instance.AddKeyboardBinding(Keys.Down, CycleDown);
            CommandManager.Instance.AddKeyboardBinding(Keys.Up, CycleUp);
            CommandManager.Instance.AddKeyboardBinding(Keys.Enter, SelectOption);
            CommandManager.Instance.AddKeyboardBinding(Keys.Escape, GoBack);

            throw new NotImplementedException();
        }

        public override void Execute(object owner, GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Exit(object owner)
        {
            throw new NotImplementedException();
        }

        public override void Draw(object owner, SpriteBatch spriteBatch)
        {
            if (currentScreen == Screen.HIGH_SCORE)
                DrawHighscore(spriteBatch);
            else
            {
                spriteBatch.DrawString(font, ">", new Vector2(viewport.TitleSafeArea.Width - 100, viewport.TitleSafeArea.Height / 2f), Color.White);
            }
        }

        public void DrawHighscore(SpriteBatch spriteBatch)
        {

        }

        #region Callbacks
        private void CycleDown(eButtonState buttonState, Vector2 intensity)
        {
            currentSelection = (Selection)(((int)currentSelection + 1) % 3);
        }

        private void CycleUp(eButtonState buttonState, Vector2 intensity)
        {
            int selectionInt = (int)currentSelection;
            selectionInt = (selectionInt - 1) >= 0 ? selectionInt - 1 : 3;
            currentSelection = (Selection)selectionInt;
        }

        private void SelectOption(eButtonState buttonState, Vector2 intensity)
        {
            switch (currentSelection)
            {
                case Selection.QUIT:
                    exitGame.Invoke();
                    break;
                case Selection.HIGH_SCORE:
                    break;
                case Selection.START:
                    StartGame = true;
                    break;
            }
        }

        private void GoBack(eButtonState buttonState, Vector2 intensity)
        {
            if (currentScreen == Screen.HIGH_SCORE)
                currentScreen = Screen.MAIN;
            else
                exitGame.Invoke();
        }
        #endregion
    }
}
