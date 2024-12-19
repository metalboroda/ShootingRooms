using UnityEngine;

namespace Assets.Scripts.FSM
{
    public interface IFiniteStateMachine
    {
        IState CurrentState { get; }
        IState PreviousState { get; }

        void Init(IState initState);
        void ChangeState(IState newState, bool overrideState = false);
        void ChangeStateWithDelay(IState newState, float delay, MonoBehaviour monoBehaviour);
    }
}