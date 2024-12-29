using Assets.Scripts.EventBus;
using Assets.Scripts.SOs.WeaponSystem;
using Assets.Scripts.WeaponSystem;
using EventBus;
using Lean.Pool;
using UnityEngine;

namespace WeaponSystem
{
    public class RangedWeapon : WeaponBase
    {
        [Space]
        [SerializeField] private Transform firePoint;
        [Space]
        [SerializeField] private ParticleSystem muzzleFlash;
        [Space]
        [SerializeField] private GameObject projectilePrefab;

        private int _currentClipCapacity;
        private int _totalAmmo;

        private RangedWeaponDataSO _rangedWeaponData;

        private void Awake()
        {
            _rangedWeaponData = WeaponData as RangedWeaponDataSO;

            if (_rangedWeaponData != null)
            {
                _currentClipCapacity = _rangedWeaponData.ClipCapacity;
                _totalAmmo = _rangedWeaponData.MaxAmmoCapacity;

                RaiseWeaponAmmoEvent();
            }
        }

        public override void Attack(Vector3 targetPosition)
        {
            if (projectilePrefab == null || firePoint == null || _currentClipCapacity <= 0) return;

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
                    WeaponID = transform.GetInstanceID(),
                });

                muzzleFlash.Play();

                GameObject projectile = LeanPool.Spawn(projectilePrefab, firePoint.position, finalRotation);

                if (projectile.TryGetComponent<Projectile>(out var projectileComponent))
                {
                    projectileComponent.Initialize(WeaponData.Damage, _rangedWeaponData.PrpojectileSpeed, WeaponData.Range);
                }
            }

            _currentClipCapacity--;

            RaiseWeaponAmmoEvent();
        }

        public void Reload()
        {
            if (_totalAmmo <= 0) return;

            int neededAmmo = _rangedWeaponData.ClipCapacity - _currentClipCapacity;
            int ammoToReload = Mathf.Min(neededAmmo, _totalAmmo);

            _currentClipCapacity += ammoToReload;
            _totalAmmo -= ammoToReload;

            RaiseWeaponAmmoEvent();
        }

        private void RaiseWeaponAmmoEvent()
        {
            EventBus<Events.WeaponAmmoEvent>.Raise(new Events.WeaponAmmoEvent
            {
                CurrentAmmo = _currentClipCapacity,
                MaxAmmo = _totalAmmo,
            });
        }
    }
}