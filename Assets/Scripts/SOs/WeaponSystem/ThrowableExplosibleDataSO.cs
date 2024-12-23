using UnityEngine;

namespace Assets.Scripts.SOs.WeaponSystem
{
    [CreateAssetMenu(fileName = "Throwable Explosible Weapon Data", menuName = "SOs/Weapon System/Throwable Explosible Weapon Data")]
    public class ThrowableExplosibleDataSO : WeaponBaseDataSO
    {
        [field: SerializeField] public int PrpojectileSpeed { get; private set; }
        [field: SerializeField] public float Spread { get; private set; }
        [field: SerializeField] public float ExplosionTime { get; private set; }
        [field: SerializeField] public float Radius { get; private set; }
        [field: SerializeField] public float ExplosionPower { get; private set; }
    }
}