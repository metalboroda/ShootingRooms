using Assets.Scripts.FSM;

namespace Assets.Scripts.Character.Enemy.States
{
  public class EnemyBaseState : State
  {
    protected EnemyController EnemyController;
    protected CharacterAnimationHandler CharacterAnimationHandler;

    public EnemyBaseState(EnemyController enemyController)
    {
      EnemyController = enemyController;
      CharacterAnimationHandler = EnemyController.CharacterAnimationHandler;
    }
  }
}