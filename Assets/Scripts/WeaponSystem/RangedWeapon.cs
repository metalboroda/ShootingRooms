using Assets.Scripts.EventBus;
using UltimatePooling;
using UnityEngine;

namespace Assets.Scripts.WeaponSystem
{
  public class RangedWeapon : WeaponBase
  {
    [SerializeField] private Transform firePoint;
    [SerializeField] private ParticleSystem muzzleFlash;
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
        float spreadAngle = Random.Range(-WeaponData.Spread, WeaponData.Spread);
        Quaternion spreadRotation = Quaternion.AngleAxis(spreadAngle, firePoint.up);
        Quaternion finalRotation = Quaternion.LookRotation(direction) * spreadRotation;

        //Debug.DrawRay(firePoint.position, direction * 10, Color.red, 2.0f);

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