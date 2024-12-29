using System.Collections;
using Assets.Scripts.Enums;
using Assets.Scripts.ImpactSystem;
using Assets.Scripts.Interfaces;
using ImpactSystem;
using Lean.Pool;
using UnityEngine;

namespace WeaponSystem
{
    public class Projectile : MonoBehaviour, IPoolable
    {
        [SerializeField] private ImpactTypeSO impactType;
        [Space]
        [SerializeField] private DamageType damageType;
        [SerializeField] private LayerMask collisionLayer;
        [Space]
        [SerializeField] private GameObject model;
        [SerializeField] private GameObject trail;

        private float _speed;
        private int _damage;
        private float _range;
        private Vector3 _startPosition;
        private readonly float _visualEnablingDelay = 0.025f;
        private readonly float _visualDisablingDelay = 0.05f;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        public void OnSpawn()
        {
            model.SetActive(false);
            trail.SetActive(false);

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            StartCoroutine(DoHandleVisual());
        }

        public void OnDespawn()
        {
            _damage = 0;
            _speed = 0;
            _range = 0;

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            model.SetActive(false);
            trail.SetActive(false);

            StopAllCoroutines();
        }

        public void Initialize(int damage, float speed, float range)
        {
            _damage = damage;
            _speed = speed;
            _range = range;
            _startPosition = transform.position;
            _rigidbody.velocity = transform.forward * _speed;
        }

        private void Update()
        {
            if (Vector3.Distance(_startPosition, transform.position) > _range)
            {
                LeanPool.Despawn(gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & collisionLayer.value) != 0)
            {
                ContactPoint contact = collision.contacts[0];
                Vector3 hitPoint = contact.point;
                Vector3 hitNormal = contact.normal;
                GameObject hitObject = collision.gameObject;

                int triangleIndex = 0;

                if (hitObject.TryGetComponent(out MeshCollider meshCollider))
                {
                    Mesh mesh = meshCollider.sharedMesh;
                    Vector3 localHitPoint = hitObject.transform.InverseTransformPoint(hitPoint);

                    triangleIndex = FindTriangleIndex(mesh, localHitPoint);
                }

                SurfaceManager.Instance.HandleImpact(
                    hitObject,
                    hitPoint,
                    hitNormal,
                    impactType,
                    triangleIndex
                );

                if (hitObject.TryGetComponent(out IDamageable damageable))
                {
                    damageable.Damage(_damage, hitPoint, damageType);
                }

                LeanPool.Despawn(gameObject);
            }
        }

        private int FindTriangleIndex(Mesh mesh, Vector3 localHitPoint)
        {
            int closestTriangleIndex = 0;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                Vector3 v0 = mesh.vertices[mesh.triangles[i]];
                Vector3 v1 = mesh.vertices[mesh.triangles[i + 1]];
                Vector3 v2 = mesh.vertices[mesh.triangles[i + 2]];

                Vector3 center = (v0 + v1 + v2) / 3f;
                float distance = Vector3.Distance(localHitPoint, center);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTriangleIndex = i / 3;
                }
            }

            return closestTriangleIndex;
        }

        private IEnumerator DoHandleVisual()
        {
            yield return new WaitForSeconds(_visualEnablingDelay);

            model.SetActive(true);
            trail.SetActive(true);

            yield return new WaitForSeconds(_visualDisablingDelay);

            model.SetActive(false);
            trail.SetActive(false);
        }

        
    }
}