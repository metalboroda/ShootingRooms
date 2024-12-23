using Assets.Scripts.EventBus;
using Assets.Scripts.SOs.WeaponSystem;
using UltimatePooling;
using UnityEngine;

namespace Assets.Scripts.WeaponSystem
{
    public class RangedWeapon : WeaponBase
    {
        [SerializeField] private RangedWeaponDataSO rangedWeaponData;
        [Space]
        [SerializeField] private Transform firePoint;
        [Space]
        [SerializeField] private ParticleSystem muzzleFlash;
        [Space]
        [SerializeField] private GameObject projectilePrefab;

        public override void Attack(Vector3 targetPosition)
        {
            if (projectilePrefab == null || firePoint == null)
            {
                return;
            }

            for (int i = 0; i < rangedWeaponData.ProjectilesPerShot; i++)
            {
                Vector3 direction = (targetPosition - firePoint.position).normalized;

                float horizontalSpread = Random.Range(-rangedWeaponData.Spread, rangedWeaponData.Spread);
                float verticalSpread = Random.Range(-rangedWeaponData.Spread, rangedWeaponData.Spread);

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
                    projectileComponent.Initialize(WeaponData.Damage, rangedWeaponData.PrpojectileSpeed, WeaponData.Range);
                }
            }
        }
    }
}