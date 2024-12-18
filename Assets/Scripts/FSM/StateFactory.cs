using System;
using System.Collections.Generic;

namespace Assets.Scripts.FSM
{
    public class StateFactory<T> where T : class
    {
        private readonly Dictionary<Type, State> _states = new Dictionary<Type, State>();
        private readonly T _context;

        public StateFactory(T context)
        {
            _context = context;
        }

        public U GetState<U>() where U : State, new()
        {
            var type = typeof(U);

            if (_states.ContainsKey(type) == false)
            {
                var state = new U();

                state.Init(_context);

                _states[type] = state;
            }

            return (U)_states[type];
        }
    }
}