namespace Assets.Scripts.Character.Enemy.States
{
    public class EnemyInjuredState : EnemyBaseState
    {
        public override void Enter()
        {
            CharacterAnimationHandler.CrossfadeAnimation(
            CharacterAnimationHandler.CharacterAnimationData.RandomDeathAnimation(), () =>
            {
                CharacterAnimationHandler.CrossfadeAnimation(
                      CharacterAnimationHandler.CharacterAnimationData.RandomInjuryAnimation());
            });

            CharacterHandler.InjuryDeath();
        }

        public override void Exit()
        {
            CharacterAnimationHandler.StopAnimationRoutine();
        }
    }
}