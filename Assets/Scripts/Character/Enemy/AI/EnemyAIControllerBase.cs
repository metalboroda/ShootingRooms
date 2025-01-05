using Assets.Scripts.EventBus;
using Assets.Scripts.FSM;
using UnityEngine;

namespace Assets.Scripts.Character.Enemy.AI
{
    public abstract class EnemyAIControllerBase : MonoBehaviour
    {
        [SerializeField] protected bool showLogs;
        
        public EnemyController EnemyController { get; protected set; }
        
        protected FiniteStateMachine stateMachine;
        protected StateFactory<EnemyAIControllerBase> stateFactory;
        
        protected EventBinding<Events.CharacterInjured> characterInjured;
        protected EventBinding<Events.CharacterDead> characterDead;
        
        protected virtual void Awake()
        {
            stateMachine = new FiniteStateMachine();
            stateFactory = new StateFactory<EnemyAIControllerBase>(this);
            EnemyController = GetComponent<EnemyController>();
        }

        protected virtual void Update()
        {
            stateMachine.CurrentState?.Update();
        }

        protected virtual void FixedUpdate()
        {
            stateMachine.CurrentState?.FixedUpdate();
        }
        
        
        protected void OnEnable()
        {
            characterDead = new EventBinding<Events.CharacterDead>(OnCharacterDead);
            EventBus<Events.CharacterDead>.Register(characterDead);
            characterInjured = new EventBinding<Events.CharacterInjured>(OnCharacterInjured);
            EventBus<Events.CharacterInjured>.Register(characterInjured);
        }

        protected void OnDisable()
        {
            EventBus<Events.CharacterDead>.Unregister(characterDead);
            EventBus<Events.CharacterInjured>.Unregister(characterInjured);
        }

        protected void OnCharacterInjured(Events.CharacterInjured characterInjured)
        {
            if (characterInjured.ID != transform.GetInstanceID()) return;
        }

        protected void OnCharacterDead(Events.CharacterDead characterDead)
        {
            if (characterDead.ID != transform.GetInstanceID()) return;
        }

        public void ChangeState<T>() where T : IState, new()
        {
            var newState = stateFactory.GetState<T>();
            if(showLogs) Debug.Log($"Enemy switch AI state from {stateMachine.CurrentState.GetType().Name} in {newState.GetType().Name}");

            newState.Init(this);
            stateMachine.ChangeState(newState);
            
        }
    }
}