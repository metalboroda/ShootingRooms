using UnityEngine;

namespace Assets.Scripts.SOs.WeaponSystem
{
    [CreateAssetMenu(fileName = "WeaponAnimationData", menuName = "SOs/Weapon System/Weapon Animation Data")]
    public class WeaponAnimationDataSO : ScriptableObject
    {
        [field: Header("Movement Settings")]
        [field: SerializeField] public float BobFrequency { get; private set; } = 5f;
        [field: SerializeField] public float BobAmplitude { get; private set; } = 0.0075f;
        [field: SerializeField] public float BobDamping { get; private set; } = 3f;
        [field: Space]
        [field: SerializeField] public float SwayAmount { get; private set; } = 0.025f;
        [field: SerializeField] public float SwaySmoothness { get; private set; } = 5f;

        [field: Header("Recoil Settings")]
        [field: SerializeField] public Vector3 RecoilAmount { get; private set; } = new Vector3(0.01f, -0.005f, -0.05f);
        [field: SerializeField] public float RecoilSpeed { get; private set; } = 0.1f;
        [field: SerializeField] public float RecoilReturnSpeed { get; private set; } = 0.1f;
        [field: Space]
        [field: SerializeField] public bool EnableShake { get; private set; }
        [field: SerializeField] public Vector3 ShakeStrength { get; private set; } = new Vector3(0.01f, 0.01f, 0);
        [field: SerializeField] public float ShakeDuration { get; private set; } = 0.2f;
        [field: SerializeField] public int ShakeVibrato { get; private set; } = 10;
        [field: SerializeField] public float ShakeRandomness { get; private set; } = 90f;
    }
}