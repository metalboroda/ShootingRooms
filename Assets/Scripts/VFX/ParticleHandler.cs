using Lean.Pool;
using UnityEngine;

namespace VFX
{
    public class ParticleHandler : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void OnParticleSystemStopped()
        {
            LeanPool.Despawn(gameObject);
        }
    }
}