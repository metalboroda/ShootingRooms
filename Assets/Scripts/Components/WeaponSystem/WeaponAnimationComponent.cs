using Assets.Scripts.SOs.WeaponSystem;
using DG.Tweening;
using UnityEngine;

namespace Components.WeaponSystem
{
    public class WeaponAnimationComponent
    {
        private readonly Transform _weaponHolder;
        private readonly Vector3 _originalWeaponPosition;
        private Vector3 _currentRecoilOffset;
        private readonly WeaponAnimationDataSO _weaponAnimationData;
        private float _bobTimer;
        private float _currentAmplitude;

        public WeaponAnimationComponent(Transform weaponHolder, WeaponAnimationDataSO weaponAnimationData)
        {
            _weaponHolder = weaponHolder;
            _originalWeaponPosition = weaponHolder.localPosition;
            _weaponAnimationData = weaponAnimationData;
        }

        public void ApplyWeaponBob(Vector2 moveDirection)
        {
            if (moveDirection != Vector2.zero)
            {
                _bobTimer += Time.deltaTime * _weaponAnimationData.BobFrequency;
                _currentAmplitude = Mathf.Lerp(
                    _currentAmplitude, _weaponAnimationData.BobAmplitude, Time.deltaTime * _weaponAnimationData.BobDamping);

                float horizontalBob = Mathf.Cos(_bobTimer) * _currentAmplitude;
                float verticalBob = Mathf.Sin(_bobTimer * 2f) * _currentAmplitude;

                Vector3 bobPosition = new Vector3(horizontalBob, verticalBob, 0);
                
                _weaponHolder.localPosition = Vector3.Lerp(
                    _weaponHolder.localPosition, _originalWeaponPosition + bobPosition + _currentRecoilOffset, Time.deltaTime * _weaponAnimationData.BobDamping);
            }
            else
            {
                _currentAmplitude = Mathf.Lerp(_currentAmplitude, 0, Time.deltaTime * _weaponAnimationData.BobDamping);
                _bobTimer += Time.deltaTime * _weaponAnimationData.BobFrequency;

                float horizontalBob = Mathf.Cos(_bobTimer) * _currentAmplitude;
                float verticalBob = Mathf.Sin(_bobTimer / 4f) * _currentAmplitude;

                Vector3 bobPosition = new Vector3(horizontalBob, verticalBob, 0);
                
                _weaponHolder.localPosition = Vector3.Lerp(
                    _weaponHolder.localPosition, _originalWeaponPosition + bobPosition + _currentRecoilOffset, Time.deltaTime * (_weaponAnimationData.BobDamping / 4f));
            }
        }

        public void ApplyWeaponSway(Vector2 lookDirection)
        {
            Vector3 swayPosition = new Vector3(-lookDirection.x, -lookDirection.y, 0) * _weaponAnimationData.SwayAmount;
            
            _weaponHolder.localPosition = Vector3.Lerp(
                _weaponHolder.localPosition, _originalWeaponPosition + swayPosition + _currentRecoilOffset, Time.deltaTime * _weaponAnimationData.SwaySmoothness);
        }

        public void ApplyRecoil()
        {
            _weaponHolder.DOKill();
            
            Sequence recoilSequence = DOTween.Sequence();

            recoilSequence.Append(_weaponHolder.DOLocalMove(_originalWeaponPosition + _weaponAnimationData.RecoilAmount, _weaponAnimationData.RecoilSpeed)
                .SetEase(Ease.OutQuad));
            recoilSequence.Append(_weaponHolder.DOLocalMove(_originalWeaponPosition, _weaponAnimationData.RecoilReturnSpeed)
                .SetEase(Ease.InQuad));

            recoilSequence.Play();
        }
    }
}
