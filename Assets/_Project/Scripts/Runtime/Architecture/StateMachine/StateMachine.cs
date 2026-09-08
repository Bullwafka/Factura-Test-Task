using System;
using System.Collections.Generic;
using UnityEngine;

namespace Factura.Gameplay.StateMachine
{
    public class StateMachine<TState> where TState : AbstractState
    {
        private Dictionary<Type, TState> _states;
        private TState _currentState;
        private TState _initialState;

        public StateMachine(TState initialState, params TState[] states)
        {
            _initialState = initialState;
            _states = new(states.Length);

            for (int i = 0; i < states.Length; i++)
                AddState(states[i]);
        }

        public void Start()
        {
            _currentState = _initialState;
        }

        public void Update()
        {
            _currentState?.Update();
        }

        public void Stop()
        {
            _currentState?.Stop();
            _currentState = null;
        }

        public void ChangeState<T>() where T : TState
        {
            if(_states.TryGetValue(typeof(T), out TState state))
            {
                _currentState?.Stop();
                _currentState = state;
                _currentState.Start();
                return;
            }

            Debug.LogError($"State {typeof(T)} was not registred");
        }

        public void AddState<T>(T state) where T : TState
        {
            Type key = typeof(T);

            if(_states.ContainsKey(key))
            {
                Debug.LogError($"State {key} has already been added");
                return;
            }

            _states.Add(key, state);
        }
    }
}
