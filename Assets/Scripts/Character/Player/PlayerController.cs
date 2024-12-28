using UnityEngine;
using Assets.Scripts.FSM;
using Assets.Scripts.Character.Player.States;

namespace Assets.Scripts.Character.Player
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerMovementHandler PlayerMovmentHandler { get; private set; }

        private FiniteStateMachine _stateMachine;
        private StateFactory<PlayerController> _stateFactory;

        private void Awake()
        {
            PlayerMovmentHandler = GetComponent<PlayerMovementHandler>();

            _stateMachine = new FiniteStateMachine();
            _stateFactory = new StateFactory<PlayerController>(this);
        }

        private void Start()
        {
            var initialState = _stateFactory.GetState<PlayerMovementState>();

            initialState.Init(this);
            _stateMachine.Init(initialState);
        }

        private void Update()
        {
            _stateMachine.CurrentState?.Update();
        }

        private void FixedUpdate()
        {
            _stateMachine.CurrentState?.FixedUpdate();
        }

        private void LateUpdate()
        {
            _stateMachine.CurrentState?.LateUpdate();
        }

        public void ChangeState<T>() where T : IState, new()
        {
            var newState = _stateFactory.GetState<T>();

            newState.Init(this);
            _stateMachine.ChangeState(newState);
        }
    }
}
