using UltimatePooling;
using UnityEngine;

namespace Assets.Scripts.VFX
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
      UltimatePool.despawn(gameObject);
    }
  }
}