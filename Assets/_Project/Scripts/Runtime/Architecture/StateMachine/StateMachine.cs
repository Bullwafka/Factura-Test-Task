using System;
using System.Collections.Generic;
using UnityEngine;

namespace Factura.Gameplay.StateMachine
{
    public class StateMachine<TState> where TState : AbstractState
    {
        private readonly Dictionary<Type, TState> _states;
        private TState _initialState;
        private TState _currentState;

        public StateMachine(TState initialState, params TState[] states)
        {
            _states = new(states.Length + 1);
            Initialize(initialState, states);
        }

        public StateMachine()
        {
            _states = new();
        }

        public void Initialize(TState initialState, params TState[] states)
        {
            _initialState = initialState ?? throw new ArgumentNullException(nameof(initialState));

            AddState(initialState);

            for (int i = 0; i < states.Length; i++)
                AddState(states[i]);
        }
        public void AddState<T>(T state) where T : TState
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            Type key = state.GetType();

            if (_states.ContainsKey(key))
            {
                Debug.LogError($"State {key} has already been added");
                return;
            }

            _states.Add(key, state);
        }

        public void Start()
        {
            if (_currentState != null)
                return;

            _currentState = _initialState;
            _currentState.Start();
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
            if (_states.TryGetValue(typeof(T), out TState state))
            {
                if (ReferenceEquals(_currentState, state))
                    return;

                _currentState?.Stop();
                _currentState = state;
                _currentState.Start();
                return;
            }

            Debug.LogError($"State {typeof(T)} was not registered");
        }
    }
}
