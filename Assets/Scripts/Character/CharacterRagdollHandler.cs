using System.Collections;
using Assets.Scripts.EventBus;
using FIMSpace.FProceduralAnimation;
using UnityEngine;

namespace Assets.Scripts.Character
{
  public class CharacterRagdollHandler : MonoBehaviour
  {
    [SerializeField] private float minRagdollActivationDelay = 0f;
    [SerializeField] private float maxRagdollActivationDelay = 0.15f;
    [SerializeField] private float minDeathMusclePower = 0.025f;
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

      StartCoroutine(DoActivateRagdoll());
      StartCoroutine(DoDecreaseMusclePowerCoroutine());
    }

    private IEnumerator DoActivateRagdoll()
    {
      float randomRagdollDelay = Random.Range(minRagdollActivationDelay, maxRagdollActivationDelay);

      yield return new WaitForSeconds(randomRagdollDelay);

      _ragdollAnimator.Actions.AnimatingMode = RagdollHandler.EAnimatingMode.Falling;
    }

    private IEnumerator DoDecreaseMusclePowerCoroutine()
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