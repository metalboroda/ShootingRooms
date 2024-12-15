using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAnimationData", menuName = "SOs/Character/Character Animation Data")]
public class CharacterAnimationDataSO : ScriptableObject
{
  public string[] DeathAnimations;

  public string RandomDeathAnimation()
  {
    return GetRandomAnimation(DeathAnimations);
  }

  private string GetRandomAnimation(string[] animations)
  {
    return animations[Random.Range(0, DeathAnimations.Length)];
  }
}