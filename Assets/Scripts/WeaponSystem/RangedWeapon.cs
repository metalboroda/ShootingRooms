using Assets.Scripts.EventBus;
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

        public override void Attack(Vector3 targetPosition)
        {
            if (projectilePrefab == null || firePoint == null)
            {
                return;
            }

            for (int i = 0; i < WeaponData.ProjectilesPerShot; i++)
            {
                Vector3 direction = (targetPosition - firePoint.position).normalized;

                float horizontalSpread = Random.Range(-WeaponData.Spread, WeaponData.Spread);
                float verticalSpread = Random.Range(-WeaponData.Spread, WeaponData.Spread);

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
                    projectileComponent.Initialize(WeaponData.Damage, WeaponData.PrpojectileSpeed, WeaponData.Range);
                }
            }
        }
    }
}