namespace Assets.Scripts.Character.Enemy.States
{
  public class EnemyMovementState : EnemyBaseState
  {
    public EnemyMovementState(EnemyController enemyController) : base(enemyController) { }

    public override void Enter()
    {
      CharacterAnimationHandler.CrossfadeAnimation(
        CharacterAnimationHandler.CharacterAnimationData.RandomIdleAnimation());
    }
  }
}