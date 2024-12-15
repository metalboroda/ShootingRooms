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

    public void CrossfadeAnimation(string animationName)
    {
      _animator.CrossFadeInFixedTime(animationName, crossfadeDuration);
    }
  }
}