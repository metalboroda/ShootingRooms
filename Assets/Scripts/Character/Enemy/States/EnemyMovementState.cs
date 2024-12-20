namespace Assets.Scripts.Character.Enemy.States
{
  public class EnemyMovementState : EnemyBaseState
  {
    public override void Enter()
    {
      CharacterAnimationHandler.CrossfadeAnimation(
        CharacterAnimationHandler.CharacterAnimationData.RandomIdleAnimation());
    }
  }
}