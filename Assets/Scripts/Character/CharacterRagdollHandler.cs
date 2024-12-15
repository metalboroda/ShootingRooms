using Assets.Scripts.EventBus;
using RootMotion.Dynamics;
using UnityEngine;

namespace Assets.Scripts.Character
{
  public class CharacterRagdollHandler : MonoBehaviour
  {
    private EventBinding<Events.CharacterDead> _characterDead;

    private PuppetMaster _puppetMaster;

    private void Awake()
    {
      _puppetMaster = GetComponentInChildren<PuppetMaster>();
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
      if (characterDead.ID != transform.GetInstanceID()) return;

      _puppetMaster.state = PuppetMaster.State.Dead;
    }
  }
}