using Assets.Scripts.Character.Enemy;
using Assets.Scripts.Character.Enemy.States;
using UnityEngine;

public class EnemyDeathState : EnemyBaseState
{
  private int _injuryPossibility = 3;

  public EnemyDeathState(EnemyController enemyController) : base(enemyController) { }

  public override void Enter()
  {
    int injuryRandom = Random.Range(0, _injuryPossibility);

    CharacterAnimationHandler.CrossfadeAnimation(
        CharacterAnimationHandler.CharacterAnimationData.RandomDeathAnimation(), () =>
        {
          if (injuryRandom == 0)
          {
            CharacterAnimationHandler.CrossfadeAnimation(
              CharacterAnimationHandler.CharacterAnimationData.RandomInjuryAnimation());
          }
        });
  }
}