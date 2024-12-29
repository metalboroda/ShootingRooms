using System.Collections;
using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;
using Assets.Scripts.SOs.WeaponSystem;
using Assets.Scripts.WeaponSystem;
using Lean.Pool;
using UnityEngine;

namespace WeaponSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class ThrowableExplosibleWeapon : WeaponBase
    {
        [Space]
        [SerializeField] private GameObject explosionPrefab;
        [Space]
        [SerializeField] private LayerMask obstacleLayer;

        private static readonly Collider[] HitColliders = new Collider[50];

        private ThrowableExplosibleDataSO _throwableExplosibleData;

        private Rigidbody _rigidbody;
        private MeshCollider _meshCollider;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _meshCollider = GetComponent<MeshCollider>();

            _throwableExplosibleData = WeaponData as ThrowableExplosibleDataSO;
        }

        private void Start()
        {
            _meshCollider.enabled = false;
        }

        public override void Attack(Vector3 targetPosition)
        {
            if (_rigidbody == null || _throwableExplosibleData == null) return;

            transform.parent = null;
            _rigidbody.isKinematic = false;

            Vector3 throwDirection = (targetPosition - transform.position).normalized;

            float horizontalSpread = Random.Range(-_throwableExplosibleData.Spread, _throwableExplosibleData.Spread);
            float verticalSpread = Random.Range(-_throwableExplosibleData.Spread, _throwableExplosibleData.Spread);

            throwDirection = Quaternion.AngleAxis(horizontalSpread, Vector3.up) * throwDirection;
            throwDirection = Quaternion.AngleAxis(verticalSpread, Vector3.right) * throwDirection;

            _rigidbody.velocity = throwDirection * _throwableExplosibleData.PrpojectileSpeed;

            StartCoroutine(DoEnableCollider());
            StartCoroutine(DoExplode());
        }

        private IEnumerator DoEnableCollider()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            _meshCollider.enabled = true;
        }

        private IEnumerator DoExplode()
        {
            yield return new WaitForSeconds(_throwableExplosibleData.ExplosionTime);

            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _throwableExplosibleData.Radius, HitColliders);

            for (int i = 0; i < hitCount; i++)
            {
                Collider hitCollider = HitColliders[i];

                float distanceToCenter = Vector3.Distance(transform.position, hitCollider.transform.position);
                float distanceFactor = Mathf.Clamp01(1 - (distanceToCenter / _throwableExplosibleData.Radius));

                Vector3 directionToTarget = (hitCollider.transform.position - transform.position).normalized;

                if (Physics.Raycast(transform.position, directionToTarget, out RaycastHit forceHit, _throwableExplosibleData.Radius, obstacleLayer))
                {
                    if (forceHit.collider != hitCollider) continue;
                }

                if (hitCollider.attachedRigidbody != null)
                {
                    hitCollider.attachedRigidbody.AddExplosionForce(
                        _throwableExplosibleData.ExplosionPower * distanceFactor,
                        transform.position,
                        _throwableExplosibleData.Radius,
                        1.0f,
                        ForceMode.Impulse
                    );
                }

                if (hitCollider.TryGetComponent<IDamageable>(out var damageable))
                {
                    if (Physics.Raycast(transform.position, directionToTarget, out RaycastHit damageHit, _throwableExplosibleData.Radius, obstacleLayer))
                    {
                        if (damageHit.collider != hitCollider) continue;
                    }

                    int scaledDamage = Mathf.RoundToInt(_throwableExplosibleData.Damage * distanceFactor);

                    damageable.Damage(scaledDamage, hitCollider.transform.position, DamageType.Explosion);
                }
            }

            GameObject spawnedExplosion = LeanPool.Spawn(explosionPrefab, transform.position, Quaternion.identity);

            spawnedExplosion.transform.parent = null;

            Destroy(gameObject);
        }
    }
}