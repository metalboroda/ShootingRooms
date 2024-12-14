using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ImpactSystem
{
  [CreateAssetMenu(menuName = "SOs/Impact System/Play Audio Effect", fileName = "PlayAudioEffect")]
  public class PlayAudioEffectSO : ScriptableObject
  {
    public AudioSource AudioSourcePrefab;
    [Tooltip("Values are clamped to 0-1")]
    public Vector2 VolumeRange = new Vector2(0, 1);
    public List<AudioClip> AudioClips = new List<AudioClip>();
  }
}