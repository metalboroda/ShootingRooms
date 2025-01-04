using Assets.Scripts.Character.Enemy.States;

public class EnemyDeathState : EnemyBaseState
{
    public override void Enter()
    {
        var animation = CharacterAnimationHandler.CharacterAnimationData.RandomDeathAnimation();
        CharacterAnimationHandler.CrossfadeAnimation(animation);

        CharacterHandler.StopAllRoutines();
    }
}