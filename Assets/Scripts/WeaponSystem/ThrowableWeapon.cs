using Assets.Scripts.SOs.WeaponSystem;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.WeaponSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class ThrowableWeapon : WeaponBase
    {
        private RangedWeaponDataSO _throwableWeaponData;

        private Rigidbody _rigidbody;
        private MeshCollider _meshCollider;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _meshCollider = GetComponent<MeshCollider>();

            _throwableWeaponData = WeaponData as RangedWeaponDataSO;
        }

        private void Start()
        {
            _meshCollider.enabled = false;
        }

        public override void Attack(Vector3 targetPosition)
        {
            if (_rigidbody == null || _throwableWeaponData == null) return;

            transform.parent = null;
            _rigidbody.isKinematic = false;

            Vector3 throwDirection = (targetPosition - transform.position).normalized;

            float horizontalSpread = Random.Range(-_throwableWeaponData.Spread, _throwableWeaponData.Spread);
            float verticalSpread = Random.Range(-_throwableWeaponData.Spread, _throwableWeaponData.Spread);

            throwDirection = Quaternion.AngleAxis(horizontalSpread, Vector3.up) * throwDirection;
            throwDirection = Quaternion.AngleAxis(verticalSpread, Vector3.right) * throwDirection;

            _rigidbody.velocity = throwDirection * _throwableWeaponData.PrpojectileSpeed;

            StartCoroutine(DoEnableCollider());
        }

        private IEnumerator DoEnableCollider()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            _meshCollider.enabled = true;
        }
    }
}