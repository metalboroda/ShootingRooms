using Assets.Scripts.FSM;
using System.Collections;
using UnityEngine;

public class FiniteStateMachine : IFiniteStateMachine
{
    public IState CurrentState { get; private set; }
    public IState PreviousState { get; private set; }

    public void Init(IState initState)
    {
        CurrentState = initState;
        CurrentState.Enter();
    }

    public void ChangeState(IState newState, bool overrideState = false)
    {
        if (overrideState == false && newState == CurrentState) return;

        PreviousState = CurrentState;
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void ChangeStateWithDelay(IState newState, float delay, MonoBehaviour monoBehaviour)
    {
        monoBehaviour.StartCoroutine(DoChangeStateWithDelay(newState, delay));
    }

    private IEnumerator DoChangeStateWithDelay(IState newState, float delay)
    {
        yield return new WaitForSeconds(delay);

        ChangeState(newState);
    }
}