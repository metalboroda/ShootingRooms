using UnityEngine;

namespace Assets.Scripts.FSM
{
    public interface IFiniteStateMachine
    {
        public IState CurrentState { get; }
        public IState PreviousState { get; }

        public void Init(IState initState);
        public void ChangeState(IState newState, bool overrideState = false);
        public void ChangeStateWithDelay(IState newState, float delay, MonoBehaviour monoBehaviour);
    }
}