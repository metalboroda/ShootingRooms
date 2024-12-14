using System.Collections;
using Assets.Scripts.EventBus;
using FIMSpace.FProceduralAnimation;
using UnityEngine;

namespace Assets.Scripts.Character
{
  public class CharacterRagdollHandler : MonoBehaviour
  {
    [SerializeField] private float minDeathMusclePower = 0.125f;
    [SerializeField] private float musclePowerDecreasingSpeed = 2.5f;

    private RagdollAnimator2 _ragdollAnimator;

    private EventBinding<Events.CharacterDead> _characterDead;

    private void Awake()
    {
      _ragdollAnimator = GetComponent<RagdollAnimator2>();
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

      _ragdollAnimator.Actions.AnimatingMode = RagdollHandler.EAnimatingMode.Falling;

      StartCoroutine(DecreaseMusclePowerCoroutine());
    }

    private IEnumerator DecreaseMusclePowerCoroutine()
    {
      while (_ragdollAnimator.Actions.MusclesPower > minDeathMusclePower)
      {
        _ragdollAnimator.Actions.MusclesPower -= musclePowerDecreasingSpeed * Time.deltaTime;

        if (_ragdollAnimator.Actions.MusclesPower < minDeathMusclePower)
        {
          _ragdollAnimator.Actions.MusclesPower = minDeathMusclePower;
        }

        _ragdollAnimator.User_UpdateAllBonesParametersAfterManualChanges();

        yield return null;
      }
    }
  }
}