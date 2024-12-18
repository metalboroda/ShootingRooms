using UnityEngine;

namespace Assets.Scripts.SOs.WeaponSystem
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "SOs/Weapon System/Weapon Data")]
    public class WeaponDataSO : ScriptableObject
    {
        [field: SerializeField] public string WeaponName { get; private set; }
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public int PrpojectileSpeed { get; private set; }
        [field: SerializeField] public float AttackRate { get; private set; }
        [field: SerializeField] public float Spread { get; private set; }
        [field: SerializeField] public int ProjectilesPerShot { get; private set; }
        [field: SerializeField] public float Range { get; private set; }

        [field: Header("")]
        [field: SerializeField] public WeaponAnimationDataSO WeaponAnimationData { get; private set; }
    }
}