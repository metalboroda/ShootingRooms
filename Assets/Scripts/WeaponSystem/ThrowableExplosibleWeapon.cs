using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;
using Assets.Scripts.SOs.WeaponSystem;
using System.Collections;
using UltimatePooling;
using UnityEngine;

namespace Assets.Scripts.WeaponSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class ThrowableExplosibleWeapon : WeaponBase
    {
        [Space]
        [SerializeField] private GameObject explosionPrefab;
        [Space]
        [SerializeField] private LayerMask obstacleLayer;

        private static readonly Collider[] _hitColliders = new Collider[50];

        private ThrowableExplosibleDataSO throwableExplosibleData;

        private Rigidbody _rigidbody;
        private MeshCollider _meshCollider;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _meshCollider = GetComponent<MeshCollider>();

            throwableExplosibleData = WeaponData as ThrowableExplosibleDataSO;
        }

        private void Start()
        {
            _meshCollider.enabled = false;
        }

        public override void Attack(Vector3 targetPosition)
        {
            if (_rigidbody == null || throwableExplosibleData == null) return;

            transform.parent = null;
            _rigidbody.isKinematic = false;

            Vector3 throwDirection = (targetPosition - transform.position).normalized;

            float horizontalSpread = Random.Range(-throwableExplosibleData.Spread, throwableExplosibleData.Spread);
            float verticalSpread = Random.Range(-throwableExplosibleData.Spread, throwableExplosibleData.Spread);

            throwDirection = Quaternion.AngleAxis(horizontalSpread, Vector3.up) * throwDirection;
            throwDirection = Quaternion.AngleAxis(verticalSpread, Vector3.right) * throwDirection;

            _rigidbody.velocity = throwDirection * throwableExplosibleData.PrpojectileSpeed;

            StartCoroutine(DoEnableCollider());
            StartCoroutine(DoExplode());
        }

        private IEnumerator DoEnableCollider()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            _meshCollider.enabled = true;
        }

        private IEnumerator DoExplode()
        {
            yield return new WaitForSeconds(throwableExplosibleData.ExplosionTime);

            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, throwableExplosibleData.Radius, _hitColliders);

            for (int i = 0; i < hitCount; i++)
            {
                Collider collider = _hitColliders[i];

                float distanceToCenter = Vector3.Distance(transform.position, collider.transform.position);
                float distanceFactor = Mathf.Clamp01(1 - (distanceToCenter / throwableExplosibleData.Radius));

                Vector3 directionToTarget = (collider.transform.position - transform.position).normalized;

                if (Physics.Raycast(transform.position, directionToTarget, out RaycastHit forceHit, throwableExplosibleData.Radius, obstacleLayer))
                {
                    if (forceHit.collider != collider) continue;
                }

                if (collider.attachedRigidbody != null)
                {
                    collider.attachedRigidbody.AddExplosionForce(
                        throwableExplosibleData.ExplosionPower * distanceFactor,
                        transform.position,
                        throwableExplosibleData.Radius,
                        1.0f,
                        ForceMode.Impulse
                    );
                }

                if (collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    if (Physics.Raycast(transform.position, directionToTarget, out RaycastHit damageHit, throwableExplosibleData.Radius, obstacleLayer))
                    {
                        if (damageHit.collider != collider) continue;
                    }

                    int scaledDamage = Mathf.RoundToInt(throwableExplosibleData.Damage * distanceFactor);

                    damageable.Damage(scaledDamage, collider.transform.position, DamageType.Explosion);
                }
            }

            GameObject spawnedExplosion = UltimatePool.spawn(explosionPrefab, transform.position, Quaternion.identity);

            spawnedExplosion.transform.parent = null;

            Destroy(gameObject);
        }
    }
}