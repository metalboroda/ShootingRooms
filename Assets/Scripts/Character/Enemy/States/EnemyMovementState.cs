using UnityEngine;

namespace Assets.Scripts.Character.Enemy.States
{
    public class EnemyMovementState : EnemyBaseState
    {
        private float _distanceToTarget;
        
        public override void Enter()
        {
            if (EnemyController.TargetPosition == null)
            {
                EnemyController.ChangeState<EnemyIdleState>();
                return;
            }
            else
            {
                NavMeshAgent?.SetDestination(EnemyController.TargetPosition.Value); 
            }
        }

        public override void FixedUpdate()
        {
            _distanceToTarget = EnemyController.transform.GetOffsetTo(EnemyController.TargetPosition.Value).sqrMagnitude;
            if (_distanceToTarget < Mathf.Pow(0.2f, 2))
            {
                EnemyController.ChangeState<EnemyIdleState>();
            }
        }

        public override void Exit()
        {
            NavMeshAgent?.Stop();
        }
    }
}