using Assets.Scripts.EventBus;
using UnityEngine;

namespace Assets.Scripts.WeaponSystem
{
  public class RangedWeaponAnimationHandler : MonoBehaviour
  {
    [SerializeField] private float crossfadeDuration = 0.1f;

    [Header("Animations")]
    [SerializeField] private string shootAnimation;

    private Animator _animator;

    private EventBinding<Events.WeaponUsed> _weaponUsed;

    private void Awake()
    {
      _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
      _weaponUsed = new EventBinding<Events.WeaponUsed>(OnWeaponUsed);
      EventBus<Events.WeaponUsed>.Register(_weaponUsed);
    }

    private void OnDisable()
    {
      EventBus<Events.WeaponUsed>.Unregister(_weaponUsed);
    }

    private void OnWeaponUsed(Events.WeaponUsed weaponUsed)
    {
      if (weaponUsed.ID != transform.GetInstanceID()) return;

      CrossfadeAnimation(shootAnimation);
    }

    private void CrossfadeAnimation(string animationName)
    {
      _animator.CrossFadeInFixedTime(animationName, crossfadeDuration);
    }
  }
}