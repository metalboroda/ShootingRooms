using Assets.Scripts.EventBus;
using Assets.Scripts.SOs.WeaponSystem;
using UltimatePooling;
using UnityEngine;

namespace Assets.Scripts.WeaponSystem
{
    public class RangedWeapon : WeaponBase
    {
        [Space]
        [SerializeField] private Transform firePoint;
        [Space]
        [SerializeField] private ParticleSystem muzzleFlash;
        [Space]
        [SerializeField] private GameObject projectilePrefab;

        private RangedWeaponDataSO _rangedWeaponData;

        private void Awake()
        {
            _rangedWeaponData = WeaponData as RangedWeaponDataSO;
        }

        public override void Attack(Vector3 targetPosition)
        {
            if (projectilePrefab == null || firePoint == null)
            {
                return;
            }

            for (int i = 0; i < _rangedWeaponData.ProjectilesPerShot; i++)
            {
                Vector3 direction = (targetPosition - firePoint.position).normalized;

                float horizontalSpread = Random.Range(-_rangedWeaponData.Spread, _rangedWeaponData.Spread);
                float verticalSpread = Random.Range(-_rangedWeaponData.Spread, _rangedWeaponData.Spread);

                Quaternion horizontalRotation = Quaternion.AngleAxis(horizontalSpread, firePoint.up);
                Quaternion verticalRotation = Quaternion.AngleAxis(verticalSpread, firePoint.right);
                Quaternion spreadRotation = horizontalRotation * verticalRotation;
                Quaternion finalRotation = Quaternion.LookRotation(direction) * spreadRotation;

                EventBus<Events.WeaponUsed>.Raise(new Events.WeaponUsed
                {
                    ID = transform.GetInstanceID(),
                });

                muzzleFlash.Play();

                GameObject projectile = UltimatePool.spawn(projectilePrefab, firePoint.position, finalRotation);

                if (projectile.TryGetComponent<Projectile>(out var projectileComponent))
                {
                    projectileComponent.Initialize(WeaponData.Damage, _rangedWeaponData.PrpojectileSpeed, WeaponData.Range);
                }
            }
        }
    }
}