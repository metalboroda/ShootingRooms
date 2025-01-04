using System.Threading;
using System.Threading.Tasks;

namespace Assets.Scripts.Character.Enemy.States
{
    public class EnemyIdleState : EnemyBaseState
    {
        private CancellationTokenSource _cts;
        
        public override void Enter()
        {
            NavMeshAgent?.Stop();
            
            var animation = CharacterAnimationHandler.CharacterAnimationData.RandomIdleAnimation();
            CharacterAnimationHandler.CrossfadeAnimation(animation);

            if (EnemyController.IsPatrol) GoToNextTarget();
        }

        public override void Exit()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private async void GoToNextTarget()
        {
            if (_cts == null || _cts.IsCancellationRequested) _cts = new();
            try
            {
                await Task.Delay(1000, _cts.Token);
                EnemyController.SetNextTargetPoint();
                EnemyController.ChangeState<EnemyMovementState>();
            }
            catch (System.OperationCanceledException)
            {
                
            }
        }
    }
}