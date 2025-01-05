using System.Collections.Generic;
using Assets.Scripts.EventBus;
using Assets.Scripts.FSM;
using ProjectDawn.Navigation.Hybrid;
using UnityEngine;

namespace Assets.Scripts.Character.Enemy.AI.Patrol
{
    public class EnemyPatrolAIController : EnemyAIControllerBase
    {
        [SerializeField] private List<Transform> enemyPath;
        public Vector3? TargetPosition { get; private set; } = null;
        public bool IsPatrol { get; private set; }
        
        private Queue<Transform> _patrolPointsQueue = new();

        protected override void Awake()
        {
            base.Awake();
            if (enemyPath != null && enemyPath.Count > 0)
            {
                enemyPath.ForEach(_ => _patrolPointsQueue.Enqueue(_));
            }
        }

        
        
        protected virtual void Start()
        { 
            /*var initialState = _stateFactory.GetState<EnemyIdleState>();

            initialState.Init(this);
            _stateMachine.Init(initialState);*/
        }


        public void SetNextTargetPoint()
        {
            var point = _patrolPointsQueue.Dequeue();
            TargetPosition = point.position;
            _patrolPointsQueue.Enqueue(point);
        }
    }
}