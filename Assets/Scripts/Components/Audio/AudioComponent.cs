using UnityEngine;

namespace Components.Audio
{
    public class AudioComponent
    {
        private float _minRandomPitch = 0.975f;
        private float _maxRandomPitch = 1.025f;

        private AudioSource _audioSource;

        public AudioComponent SetAudioSource(AudioSource audioSource)
        {
            _audioSource = audioSource;
            return this;
        }

        public AudioComponent SetRandomPitch(float minRandomPitch, float maxRandomPitch)
        {
            _minRandomPitch = minRandomPitch;
            _maxRandomPitch = maxRandomPitch;
            return this;
        }

        public void PlayClipOneShot(AudioClip audioClip, bool randomPitch = false)
        {
            var newPitch = Random.Range(_minRandomPitch, _maxRandomPitch);

            if (randomPitch == true)
            {
                _audioSource.pitch = newPitch;
            }

            _audioSource.PlayOneShot(audioClip);
        }
    }
}