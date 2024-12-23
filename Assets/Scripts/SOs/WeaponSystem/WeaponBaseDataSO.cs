using UnityEngine;

namespace Assets.Scripts.SOs.WeaponSystem
{
    public class WeaponBaseDataSO : ScriptableObject
    {
        [field: SerializeField] public string WeaponName { get; private set; }
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public float AttackRate { get; private set; }
        [field: SerializeField] public float Range { get; private set; }

        [field: Header("Animation")]
        [field: SerializeField] public WeaponAnimationDataSO WeaponAnimationData { get; private set; }
    }
}