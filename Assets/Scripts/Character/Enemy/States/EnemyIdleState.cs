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
        }
    }
}