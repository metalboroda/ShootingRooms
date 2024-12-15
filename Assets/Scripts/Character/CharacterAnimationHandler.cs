using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Character
{
  public class CharacterAnimationHandler : MonoBehaviour
  {
    [SerializeField] private float crossfadeDuration = 0.15f;

    [field: Header("")]
    [field: SerializeField] public CharacterAnimationDataSO CharacterAnimationData { get; private set; }

    private Animator _animator;

    private void Awake()
    {
      _animator = GetComponent<Animator>();
    }

    public void CrossfadeAnimation(string animationName, Action onComplete = null)
    {
      _animator.CrossFadeInFixedTime(animationName, crossfadeDuration);

      if (onComplete != null)
      {
        StartCoroutine(WaitForAnimationToEnd(animationName, onComplete));
      }
    }

    private IEnumerator WaitForAnimationToEnd(string animationName, Action onComplete)
    {
      yield return new WaitForEndOfFrame();

      var currentState = _animator.GetCurrentAnimatorStateInfo(0);

      while (currentState.IsName(animationName) == false)
      {
        currentState = _animator.GetCurrentAnimatorStateInfo(0);
        yield return null;
      }

      while (currentState.normalizedTime < 1.0f)
      {
        currentState = _animator.GetCurrentAnimatorStateInfo(0);
        yield return null;
      }

      onComplete?.Invoke();
    }
  }
}