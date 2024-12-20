using Assets.Scripts.Character.Enemy.States;
using Assets.Scripts.EventBus;
using Assets.Scripts.FSM;
using UnityEngine;

namespace Assets.Scripts.Character.Enemy
{
    [SelectionBase]
    public class EnemyController : MonoBehaviour
    {
        public CharacterHandler CharacterHandler { get; private set; }
        public CharacterAnimationHandler CharacterAnimationHandler { get; private set; }

        private FiniteStateMachine _stateMachine;
        private StateFactory<EnemyController> _stateFactory;

        private EventBinding<Events.CharacterInjured> _characterInjured;
        private EventBinding<Events.CharacterDead> _characterDead;

        private void Awake()
        {
            CharacterHandler = GetComponent<CharacterHandler>();
            CharacterAnimationHandler = GetComponent<CharacterAnimationHandler>();

            _stateMachine = new FiniteStateMachine();
            _stateFactory = new StateFactory<EnemyController>(this);
        }

        private void OnEnable()
        {
            _characterDead = new EventBinding<Events.CharacterDead>(OnCharacterDead);
            EventBus<Events.CharacterDead>.Register(_characterDead);
            _characterInjured = new EventBinding<Events.CharacterInjured>(OnCharacterInjured);
            EventBus<Events.CharacterInjured>.Register(_characterInjured);
        }

        private void OnDisable()
        {
            EventBus<Events.CharacterDead>.Unregister(_characterDead);
            EventBus<Events.CharacterInjured>.Unregister(_characterInjured);
        }

        private void Start()
        {
            var initialState = _stateFactory.GetState<EnemyMovementState>();

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

        private void OnCharacterInjured(Events.CharacterInjured characterInjured)
        {
            if (characterInjured.ID != transform.GetInstanceID()) return;

            if (_stateMachine.CurrentState is not EnemyInjuredState)
            {
                ChangeState<EnemyInjuredState>();
            }
        }

        private void OnCharacterDead(Events.CharacterDead characterDead)
        {
            if (characterDead.ID != transform.GetInstanceID()) return;

            if (_stateMachine.CurrentState is not EnemyDeathState)
            {
                ChangeState<EnemyDeathState>();
            }
        }

        public void ChangeState<T>() where T : IState, new()
        {
            var newState = _stateFactory.GetState<T>();

            newState.Init(this);
            _stateMachine.ChangeState(newState);

            Debug.Log("Enemy State" +  newState);
        }
    }
}