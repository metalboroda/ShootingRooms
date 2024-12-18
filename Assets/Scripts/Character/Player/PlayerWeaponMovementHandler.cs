using Assets.Scripts.SOs.WeaponSystem;
using DG.Tweening;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace Assets.Scripts.Character.Player
{
    public class PlayerWeaponMovementHandler : MonoBehaviour
    {
        [SerializeField] private Transform weaponHolder;

        private Vector3 _originalWeaponPosition;
        private float _bobTimer;
        private float _currentAmplitude;
        private Vector3 _currentRecoilOffset;

        private PlayerWeaponHandler _weaponHandler;
        private WeaponDataSO _weaponData;
        private WeaponAnimationDataSO _weaponAnimationData;

        private void Awake()
        {
            _weaponHandler = GetComponent<PlayerWeaponHandler>();

            if (weaponHolder != null)
            {
                _originalWeaponPosition = weaponHolder.localPosition;
            }
        }

        private void Start()
        {
            _weaponData = _weaponHandler.Weapon.WeaponData;
            _weaponAnimationData = _weaponData.WeaponAnimationData;
        }

        public void ApplyWeaponBob(float horizontalInput, float verticalInput)
        {
            if (horizontalInput != 0 || verticalInput != 0)
            {
                _bobTimer += Time.deltaTime * _weaponAnimationData.BobFrequency;
                _currentAmplitude = Mathf.Lerp(
                    _currentAmplitude, _weaponAnimationData.BobAmplitude, Time.deltaTime * _weaponAnimationData.BobDamping);

                float horizontalBob = Mathf.Cos(_bobTimer) * _currentAmplitude;
                float verticalBob = Mathf.Sin(_bobTimer * 2) * _currentAmplitude;

                Vector3 bobPosition = new Vector3(horizontalBob, verticalBob, 0);

                weaponHolder.localPosition = Vector3.Lerp(
                    weaponHolder.localPosition, _originalWeaponPosition + bobPosition + _currentRecoilOffset, Time.deltaTime * _weaponAnimationData.BobDamping);
            }
            else
            {
                _currentAmplitude = Mathf.Lerp(_currentAmplitude, 0, Time.deltaTime * _weaponAnimationData.BobDamping);
                _bobTimer += Time.deltaTime * _weaponAnimationData.BobFrequency;

                float horizontalBob = Mathf.Cos(_bobTimer) * _currentAmplitude;
                float verticalBob = Mathf.Sin(_bobTimer * 2) * _currentAmplitude;

                Vector3 bobPosition = new Vector3(horizontalBob, verticalBob, 0);

                weaponHolder.localPosition = Vector3.Lerp(
                    weaponHolder.localPosition, _originalWeaponPosition + bobPosition + _currentRecoilOffset, Time.deltaTime * _weaponAnimationData.BobDamping);
            }
        }

        public void ApplyWeaponSway(float mouseX, float mouseY)
        {
            Vector3 swayPosition = new Vector3(-mouseX, -mouseY, 0) * _weaponAnimationData.SwayAmount;

            weaponHolder.localPosition = Vector3.Lerp(
                weaponHolder.localPosition, _originalWeaponPosition + swayPosition + _currentRecoilOffset, Time.deltaTime * _weaponAnimationData.SwaySmoothness);
        }

        public void ApplyRecoil()
        {
            weaponHolder.DOKill();

            Sequence recoilSequence = DOTween.Sequence();

            recoilSequence.Append(weaponHolder.DOLocalMove(_originalWeaponPosition + _weaponAnimationData.RecoilAmount, _weaponAnimationData.RecoilSpeed)
                .SetEase(Ease.OutQuad));
            recoilSequence.Append(weaponHolder.DOLocalMove(_originalWeaponPosition, _weaponAnimationData.RecoilReturnSpeed)
                .SetEase(Ease.InQuad));

            if (_weaponAnimationData.EnableShake == true)
            {
                weaponHolder.DOShakePosition(
                    _weaponAnimationData.ShakeDuration, _weaponAnimationData.ShakeStrength, _weaponAnimationData.ShakeVibrato, _weaponAnimationData.ShakeRandomness, fadeOut: false);
            }

            recoilSequence.Play();
        }
    }
}