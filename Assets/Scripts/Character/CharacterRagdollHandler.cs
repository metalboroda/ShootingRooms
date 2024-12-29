using Assets.Scripts.EventBus;
using EventBus;
using RootMotion.Dynamics;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class CharacterRagdollHandler : MonoBehaviour
    {
        [Header("Injury Settings")]
        [SerializeField] private float injuryPinWeight = 0.3f;

        private PuppetMaster _puppetMaster;

        private EventBinding<Events.CharacterInjured> _characterInjured;
        private EventBinding<Events.CharacterDead> _characterDead;

        private void Awake()
        {
            _puppetMaster = GetComponentInChildren<PuppetMaster>();
        }

        private void OnEnable()
        {
            _characterInjured = new EventBinding<Events.CharacterInjured>(OnCharacterInjured);
            EventBus<Events.CharacterInjured>.Register(_characterInjured);
            _characterDead = new EventBinding<Events.CharacterDead>(OnCharacterDead);
            EventBus<Events.CharacterDead>.Register(_characterDead);
        }

        private void OnDisable()
        {
            EventBus<Events.CharacterInjured>.Unregister(_characterInjured);
            EventBus<Events.CharacterDead>.Unregister(_characterDead);
        }

        private void OnCharacterInjured(Events.CharacterInjured characterInjured)
        {
            if (characterInjured.ID != transform.GetInstanceID()) return;

            _puppetMaster.pinWeight = injuryPinWeight;
            _puppetMaster.internalCollisions = true;
        }

        private void OnCharacterDead(Events.CharacterDead characterDead)
        {
            if (_puppetMaster.state == PuppetMaster.State.Dead) return;
            if (characterDead.ID != transform.GetInstanceID()) return;

            _puppetMaster.state = PuppetMaster.State.Dead;
        }
    }
}