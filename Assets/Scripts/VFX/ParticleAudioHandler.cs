using Components.Audio;
using Lean.Pool;
using UnityEngine;

namespace VFX
{
    [RequireComponent(typeof(AudioSource))]
    public class ParticleAudioHandler : MonoBehaviour, IPoolable
    {
        [SerializeField] private AudioClip effectClip;

        private AudioComponent _audioComponent;

        private void Awake()
        {
            _audioComponent = new AudioComponent().SetAudioSource(GetComponent<AudioSource>());
        }

        public void OnSpawn()
        {
            _audioComponent.PlayClipOneShot(effectClip, true);
        }

        public void OnDespawn()
        {
        }
    }
}