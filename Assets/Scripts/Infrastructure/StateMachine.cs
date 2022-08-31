using System;
using System.Collections.Generic;

namespace Infrastructure
{
    public class StateMachine : IStateMachine
    {
        private Dictionary<Type, IState> _states = new Dictionary<Type, IState>();

        private IState _currentState;

        public void RegisterStates(params IState[] states)
        {
            for (int i = 0; i < states.Length; i++) 
                _states.Add(states[i].GetType(), states[i]);
        }
        
        public void ChangeState<TState>() where TState : IState
        {
            if(_currentState != null)
                _currentState.Exit();

            if (_states.TryGetValue(typeof(TState), out IState foundState))
            {
                _currentState = foundState;
                _currentState.Enter();
                
                return;
            }

            throw new Exception("State not found!");
        }
    }
}