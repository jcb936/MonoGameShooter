using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace THClone
{
    public delegate void GameAction(eButtonState buttonState, Vector2 amount);

    class CommandManager
    {
        // Make it a singleton
        public static CommandManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CommandManager();
                return _instance;
            }
        }

        private static CommandManager _instance;

        private InputListener m_Input;

        private Dictionary<Keys, GameAction> m_KeyBindings = new Dictionary<Keys, GameAction>();
        private Dictionary<MouseButton, GameAction> m_MouseButtonBindings = new Dictionary<MouseButton, GameAction>();

        public CommandManager()
        {
            m_Input = new InputListener();

            // Register events with the input listener
            m_Input.OnKeyDown += this.OnKeyDown;
            m_Input.OnKeyPressed += this.OnKeyPressed;
            m_Input.OnKeyUp += this.OnKeyUp;
            m_Input.OnMouseButtonDown += this.OnMouseButtonDown;
        }

        public void Update()
        {
            // Update polling input listener, everything else is handled by events
            m_Input.Update();
        }

        public void OnKeyDown(object sender, KeyboardEventArgs e)
        {
            m_KeyBindings[e.Key]?.Invoke(eButtonState.DOWN, new Vector2(1.0f));
        }

        public void OnKeyUp(object sender, KeyboardEventArgs e)
        {
            m_KeyBindings[e.Key]?.Invoke(eButtonState.UP, new Vector2(1.0f));
        }

        public void OnKeyPressed(object sender, KeyboardEventArgs e)
        {
            m_KeyBindings[e.Key]?.Invoke(eButtonState.PRESSED, new Vector2(1.0f));
        }

        public void OnMouseButtonDown(object sender, MouseEventArgs e)
        {
            m_MouseButtonBindings[e.Button]?.Invoke(eButtonState.DOWN, new Vector2(e.CurrentState.X, e.CurrentState.Y));
        }

        public void AddKeyboardBinding(Keys key, GameAction action)
        {
            // Add key to listen for when polling
            m_Input.AddKey(key);

            // Add the binding to the command map
            m_KeyBindings[key] = action;
        }

        public void RemoveKeyboardBinding(Keys key)
        {
            // Add the binding to the command map
            m_KeyBindings[key] = null;
        }

        public void AddMouseBinding(MouseButton button, GameAction action)
        {
            // Add key to listen for when polling
            m_Input.AddButton(button);

            // Add the binding to the command map
            m_MouseButtonBindings.Add(button, action);
        }

    }
}
