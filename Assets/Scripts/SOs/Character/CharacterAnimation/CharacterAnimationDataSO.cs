using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAnimationData", menuName = "SOs/Character/Character Animation Data")]
public class CharacterAnimationDataSO : ScriptableObject
{
  public string[] IdleAnimations;
  public string[] DeathAnimations;
  public string[] InjuryAnimations;

  public string RandomIdleAnimation()
  {
    return GetRandomAnimation(IdleAnimations);
  }

  public string RandomDeathAnimation()
  {
    return GetRandomAnimation(DeathAnimations);
  }

  public string RandomInjuryAnimation()
  {
    return GetRandomAnimation(InjuryAnimations);
  }

  private string GetRandomAnimation(string[] animations)
  {
    return animations[Random.Range(0, animations.Length)];
  }
}