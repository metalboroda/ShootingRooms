namespace Assets.Scripts.Character.Enemy.States
{
    public class EnemyInjuredState : EnemyBaseState
    {
        public override void Enter()
        {
            var animation = CharacterAnimationHandler.CharacterAnimationData.RandomDeathAnimation();
            CharacterAnimationHandler.CrossfadeAnimation(animation, () =>
            {
                animation = CharacterAnimationHandler.CharacterAnimationData.RandomInjuryAnimation();
                CharacterAnimationHandler.CrossfadeAnimation(animation);
            });

            CharacterHandler.InjuryDeath();
        }

        public override void Exit()
        {
            CharacterAnimationHandler.StopAnimationRoutine();
        }
    }
}