using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ImpactSystem
{
  [CreateAssetMenu(menuName = "SOs/Impact System/Surface Effect", fileName = "SurfaceEffect")]
  public class SurfaceEffectSO : ScriptableObject
  {
    public List<SpawnObjectEffectSO> SpawnObjectEffects = new List<SpawnObjectEffectSO>();
    public List<PlayAudioEffectSO> PlayAudioEffects = new List<PlayAudioEffectSO>();
  }
}