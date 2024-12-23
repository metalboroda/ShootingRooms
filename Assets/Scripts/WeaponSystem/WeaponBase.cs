using Assets.Scripts.SOs.WeaponSystem;
using UnityEngine;

namespace Assets.Scripts.WeaponSystem
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [field: SerializeField] public WeaponBaseDataSO WeaponData { get; private set; }
        [field: SerializeField] public WeaponAnimationDataSO WeaponAnimationData { get; private set; }

        private float nextAttackTime = 0f;

        public void TryAttack(Vector3 targetPosition)
        {
            if (Time.time >= nextAttackTime)
            {
                Attack(targetPosition);

                nextAttackTime = Time.time + WeaponData.AttackRate;
            }
        }

        public abstract void Attack(Vector3 targetPosition);
    }
}