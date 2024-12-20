using Assets.Scripts.Character.Enemy.States;

public class EnemyDeathState : EnemyBaseState
{
    public override void Enter()
    {
        CharacterAnimationHandler.CrossfadeAnimation(
            CharacterAnimationHandler.CharacterAnimationData.RandomDeathAnimation());

        CharacterHandler.StopAllRoutines();
    }
}