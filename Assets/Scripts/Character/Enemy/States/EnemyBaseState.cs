using Assets.Scripts.FSM;
using Character;

namespace Assets.Scripts.Character.Enemy.States
{
    public class EnemyBaseState : State
    {
        protected EnemyController EnemyController;
        protected CharacterHandler CharacterHandler;
        protected CharacterAnimationHandler CharacterAnimationHandler;

        public override void Init(object context)
        {
            EnemyController = context as EnemyController;
            CharacterHandler = EnemyController.CharacterHandler;
            CharacterAnimationHandler = EnemyController.CharacterAnimationHandler;
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
        }

        public override void FixedUpdate()
        {
        }
    }
}