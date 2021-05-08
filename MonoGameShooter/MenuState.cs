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

        private ContentManager manager;

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

        private float currentPointerYOffset;

        private Action exitGame;

        private Viewport viewport;

        public void Initialize(Viewport viewport)
        {
            manager = new(GameInfo.GameInstance.Services, GameInfo.GameInstance.Content.RootDirectory);
            Name = "MenuState";
            this.viewport = viewport;
        }

        public void LoadContent()
        {
            backgroundImage = manager.Load<Texture2D>("Graphics\\t_mainMenu");
            font = manager.Load<SpriteFont>("Graphics\\gameFont");
        }

        public void UnloadContent()
        {
            manager.Unload();
        }

        public override void Enter(object owner, State prevState)
        {
            LoadContent();

            currentSelection = Selection.START;
            currentScreen = Screen.MAIN;
            currentPointerYOffset = 0f;
            StartGame = false;
            if (owner.GetType() == typeof(Game))
                exitGame = () => (owner as Game).Exit();

            CommandManager.Instance.AddKeyboardBinding(Keys.Down, CycleDown);
            CommandManager.Instance.AddKeyboardBinding(Keys.Up, CycleUp);
            CommandManager.Instance.AddKeyboardBinding(Keys.Enter, SelectOption);
            CommandManager.Instance.AddKeyboardBinding(Keys.Escape, GoBack);
        }

        public override void Execute(object owner, GameTime gameTime)
        {
        }

        public override void Exit(object owner, State nextState)
        {
            CommandManager.Instance.RemoveKeyboardBinding(Keys.Down);
            CommandManager.Instance.RemoveKeyboardBinding(Keys.Up);
            CommandManager.Instance.RemoveKeyboardBinding(Keys.Enter);
            CommandManager.Instance.RemoveKeyboardBinding(Keys.Escape);

            UnloadContent();
        }

        public override void Draw(object owner, SpriteBatch spriteBatch)
        {
            if (currentScreen == Screen.HIGH_SCORE)
                DrawHighscore(spriteBatch);
            else
            {
                var rect = new Rectangle(0, 0, viewport.Width, viewport.Height);
                spriteBatch.Draw(backgroundImage, rect, Color.White);
                spriteBatch.DrawString(font, ">", new Vector2(viewport.TitleSafeArea.Width - 320, (viewport.TitleSafeArea.Height / 2f - 20) + currentPointerYOffset), Color.White);
                DrawHighscore(spriteBatch);
            }
        }

        public void DrawHighscore(SpriteBatch spriteBatch)
        {
            Leaderboard leaderboard = GameInfo.GetHighscores();
            spriteBatch.DrawString(font, "HighScore", new Vector2(50, viewport.TitleSafeArea.Height / 2f + 100), Color.White);
            spriteBatch.DrawString(font, leaderboard.Entry.Name + ": " + leaderboard.Entry.Score, new Vector2(50, viewport.TitleSafeArea.Height / 2f + 150), Color.White);
        }

        private void SetPointerLocation()
        {
            switch (currentSelection)
            {
                case Selection.START:
                    currentPointerYOffset = -80f;
                    break;
                case Selection.HIGH_SCORE:
                    currentPointerYOffset = 0;
                    break;
                case Selection.QUIT:
                    currentPointerYOffset = 80f;
                    break;
            }
        }

        #region Callbacks
        private void CycleDown(eButtonState buttonState, Vector2 intensity)
        {
            if (buttonState == eButtonState.PRESSED)
            {
                currentSelection = (Selection)(((int)currentSelection + 1) % 3);

                SetPointerLocation();
            }
        }

        private void CycleUp(eButtonState buttonState, Vector2 intensity)
        {
            if (buttonState == eButtonState.PRESSED)
            {
                int selectionInt = (int)currentSelection;
                selectionInt = (selectionInt - 1) >= 0 ? selectionInt - 1 : 3;
                currentSelection = (Selection)selectionInt;

                SetPointerLocation();

            }
        }

        private void SelectOption(eButtonState buttonState, Vector2 intensity)
        {
            if (buttonState == eButtonState.PRESSED)
            {
                switch (currentSelection)
                {
                    case Selection.QUIT:
                        GameInfo.ExitGame();
                        break;
                    case Selection.HIGH_SCORE:
                        break;
                    case Selection.START:
                        StartGame = true;
                        break;
                }

            }

        }

        private void GoBack(eButtonState buttonState, Vector2 intensity)
        {
            if (currentScreen == Screen.HIGH_SCORE)
                currentScreen = Screen.MAIN;
            else
                GameInfo.ExitGame();
        }
        #endregion
    }
}
