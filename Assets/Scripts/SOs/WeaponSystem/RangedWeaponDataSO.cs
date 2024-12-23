using UnityEngine;

namespace Assets.Scripts.SOs.WeaponSystem
{
    [CreateAssetMenu(fileName = "Ranged Weapon Data", menuName = "SOs/Weapon System/Ranged Weapon Data")]
    public class RangedWeaponDataSO : WeaponBaseDataSO
    {
        [field: SerializeField] public int PrpojectileSpeed { get; private set; }
        [field: SerializeField] public float Spread { get; private set; }
        [field: SerializeField] public int ProjectilesPerShot { get; private set; }
    }
}