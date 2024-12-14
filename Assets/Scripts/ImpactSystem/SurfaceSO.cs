using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Scripts.ImpactSystem
{
  [CreateAssetMenu(menuName = "SOs/Impact System/Surface", fileName = "Surface")]
  public class SurfaceSO : ScriptableObject
  {
    [Serializable]
    public class SurfaceImpactTypeEffect
    {
      public ImpactTypeSO ImpactType;
      public SurfaceEffectSO SurfaceEffect;
    }

    public List<SurfaceImpactTypeEffect> ImpactTypeEffects = new List<SurfaceImpactTypeEffect>();
  }
}