using Assets.Scripts.EventBus;
using UnityEngine;

namespace Assets.Scripts.Character
{
  public class CharacterAnimationHandler : MonoBehaviour
  {
    [SerializeField] private float crossfadeDuration = 0.15f;

    [Header("")]
    [SerializeField] private CharacterAnimationDataSO characterAnimationData;

    private bool _isDead;

    private Animator _animator;

    private EventBinding<Events.CharacterDead> _characterDead;

    private void Awake()
    {
      _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
      _characterDead = new EventBinding<Events.CharacterDead>(OnCharacterDead);
      EventBus<Events.CharacterDead>.Register(_characterDead);
    }

    private void OnDisable()
    {
      EventBus<Events.CharacterDead>.Unregister(_characterDead);
    }

    private void OnCharacterDead(Events.CharacterDead characterDead)
    {
      if (characterDead.ID != transform.GetInstanceID())
      {
        return;
      }

      if (_isDead == true)
      {
        return;
      }

      _animator.CrossFadeInFixedTime(characterAnimationData.RandomDeathAnimation(), crossfadeDuration);

      _isDead = true;
    }
  }
}