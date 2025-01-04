using System.Collections.Generic;
using Assets.Scripts.Character.Enemy.States;
using Assets.Scripts.EventBus;
using Assets.Scripts.FSM;
using ProjectDawn.Navigation.Hybrid;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Character.Enemy
{
    [SelectionBase]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private List<Transform> enemyPath;
        [SerializeField] private bool showLogs;
        
        public CharacterHandler CharacterHandler { get; private set; }
        public CharacterAnimationHandler CharacterAnimationHandler { get; private set; }
        public AgentAuthoring NavMeshAgent { get; private set; }
        public Vector3? TargetPosition { get; private set; } = null;
        public bool IsPatrol { get; private set; }

        private FiniteStateMachine _stateMachine;
        private StateFactory<EnemyController> _stateFactory;

        private EventBinding<Events.CharacterInjured> _characterInjured;
        private EventBinding<Events.CharacterDead> _characterDead;
        
        private Queue<Transform> _patrolPointsQueue = new();

        private void Awake()
        {
            NavMeshAgent = GetComponent<AgentAuthoring>();
            CharacterHandler = GetComponent<CharacterHandler>();
            CharacterAnimationHandler = GetComponent<CharacterAnimationHandler>();

            _stateMachine = new FiniteStateMachine();
            _stateFactory = new StateFactory<EnemyController>(this);
            
            IsPatrol = enemyPath != null && enemyPath.Count > 0;
            if(IsPatrol) enemyPath.ForEach(_ => _patrolPointsQueue.Enqueue(_));
        }

        private void Start()
        {
            var initialState = _stateFactory.GetState<EnemyIdleState>();

            initialState.Init(this);
            _stateMachine.Init(initialState);
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
            if(showLogs) Debug.Log($"Enemy switch state from {_stateMachine.CurrentState.GetType().Name} in {newState.GetType().Name}");

            newState.Init(this);
            _stateMachine.ChangeState(newState);
            
        }

        public void SetNextTargetPoint()
        {
            var point = _patrolPointsQueue.Dequeue();
            TargetPosition = point.position;
            _patrolPointsQueue.Enqueue(point);
        }
    }
}