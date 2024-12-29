using Assets.Scripts.EventBus;
using Components.Audio;
using UnityEngine;

namespace WeaponSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class WeaponAudioHandler : MonoBehaviour
    {
        [SerializeField] private AudioClip shotClip;

        private AudioComponent _audioComponent;

        private EventBinding<Events.WeaponUsed> _weaponUsed;

        private void Awake()
        {
            _audioComponent = new AudioComponent()
                .SetAudioSource(GetComponent<AudioSource>());
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

            _audioComponent.PlayClipOneShot(shotClip, true);
        }
    }
}