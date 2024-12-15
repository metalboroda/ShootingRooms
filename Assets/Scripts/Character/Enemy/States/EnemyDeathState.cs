using Assets.Scripts.Character.Enemy;
using Assets.Scripts.Character.Enemy.States;

public class EnemyDeathState : EnemyBaseState
{
  public EnemyDeathState(EnemyController enemyController) : base(enemyController) { }

  public override void Enter()
  {
    CharacterAnimationHandler.CrossfadeAnimation(
        CharacterAnimationHandler.CharacterAnimationData.RandomDeathAnimation());
    CharacterAnimationHandler.HandleInjury();
  }
}
