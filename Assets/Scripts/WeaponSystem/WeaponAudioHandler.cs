using Assets.Scripts.EventBus;
using UnityEngine;

namespace Assets.Scripts.WeaponSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class WeaponAudioHandler : MonoBehaviour
    {
        [SerializeField] private AudioClip shotClip;

        private AudioSource _audioSource;

        private EventBinding<Events.WeaponUsed> _weaponUsed;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
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

            PlayClipOneShot(shotClip, true);
        }

        private void PlayClipOneShot(AudioClip audioClip, bool randomPitch = false)
        {
            float newPitch = Random.Range(0.99f, 1.01f);

            if (randomPitch == true)
            {
                _audioSource.pitch = newPitch;
            }

            _audioSource.PlayOneShot(audioClip);
        }
    }
}