﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace THClone
{
    public abstract class State
    {
        public abstract void Enter(object owner, State previousState = null);
        public abstract void Exit(object owner, State nextState = null);
        public abstract void Execute(object owner, GameTime gameTime);

        public virtual void Draw(object owner, SpriteBatch spriteBatch) { }

        public string Name
        {
            get;
            set;
        }

        private List<Transition> m_Transitions = new List<Transition>();
        public List<Transition> Transitions
        {
            get { return m_Transitions; }
        }

        public void AddTransition(Transition transition)
        {
            m_Transitions.Add(transition);
        }

    }

    public class Transition
    {
        public readonly State NextState;
        public readonly Func<bool> Condition;

        public Transition(State nextState, Func<bool> condition)
        {
            NextState = nextState;
            Condition = condition;
        }
    }


}
