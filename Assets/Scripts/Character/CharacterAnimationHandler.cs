using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Character
{
  public class CharacterAnimationHandler : MonoBehaviour
  {
    [SerializeField] private float crossfadeDuration = 0.15f;

    [Header("Injury Settings")]
    [SerializeField] private float injuryDelay = 2f;
    [SerializeField] private float injuryDuration = 5f;

    [field: Header("")]
    [field: SerializeField] public CharacterAnimationDataSO CharacterAnimationData { get; private set; }

    private int _injuryPossibility = 3;

    private Animator _animator;
    private CharacterRagdollHandler _characterRagdollHandler;

    private void Awake()
    {
      _animator = GetComponent<Animator>();
      _characterRagdollHandler = GetComponent<CharacterRagdollHandler>();
    }

    public void HandleInjury()
    {
      int randomPossibility = Random.Range(0, _injuryPossibility);

      if (randomPossibility == 0)
      {
        StartCoroutine(DoHandleInjury(true));
      }

      StartCoroutine(DoHandleInjury(false));
    }

    private IEnumerator DoHandleInjury(bool playAnimation)
    {
      yield return new WaitForSeconds(injuryDelay);

      if (playAnimation == true)
      {
        CrossfadeAnimation(CharacterAnimationData.RandomInjuryAnimation());
      }

      yield return new WaitForSeconds(injuryDuration);

      yield return new WaitForSeconds(injuryDuration / 2);

      _animator.enabled = false;
    }

    public void CrossfadeAnimation(string animationName)
    {
      _animator.CrossFadeInFixedTime(animationName, crossfadeDuration);
    }
  }
}