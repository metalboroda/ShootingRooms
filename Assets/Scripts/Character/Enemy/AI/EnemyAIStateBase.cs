using Assets.Scripts.FSM;

namespace Assets.Scripts.Character.Enemy.AI
{
    public class EnemyAIStateBase : State
    {
        protected EnemyAIControllerBase AIController;
        protected EnemyController EnemyController;
        protected CharacterHandler CharacterHandler;
        protected CharacterAnimationHandler CharacterAnimationHandler;

        public override void Init(object context)
        {
            AIController = context as EnemyAIControllerBase;
            EnemyController = AIController.EnemyController;
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