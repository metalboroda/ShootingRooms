using DG.Tweening;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace Assets.Scripts.Character.Player
{
    public class PlayerWeaponMovementHandler : MonoBehaviour
    {
        [Header("Weapon Movement Settings")]
        [SerializeField] private Transform weaponHolder;
        [Space]
        [SerializeField] private float bobFrequency = 5f;
        [SerializeField] private float bobAmplitude = 0.0075f;
        [SerializeField] private float bobDamping = 3f;
        [Space]
        [SerializeField] private float swayAmount = 0.025f;
        [SerializeField] private float swaySmoothness = 5f;
        [Space]
        [Header("Weapon Recoil Settings")]
        [SerializeField] private Vector3 recoilAmount = new Vector3(0.01f, -0.005f, -0.05f);
        [SerializeField] private float recoilSpeed = 0.1f;
        [SerializeField] private float recoilReturnSpeed = 0.1f;
        [Space]
        [SerializeField] private Vector3 shakeStrength = new Vector3(0.01f, 0.01f, 0);
        [SerializeField] private float shakeDuration = 0.2f;
        [SerializeField] private int shakeVibrato = 10;
        [SerializeField] private float shakeRandomness = 90f;

        private Vector3 _originalWeaponPosition;
        private float _bobTimer;
        private float _currentAmplitude;
        private Vector3 _currentRecoilOffset;

        private void Awake()
        {
            if (weaponHolder != null)
            {
                _originalWeaponPosition = weaponHolder.localPosition;
            }
        }

        public void ApplyWeaponBob(float horizontalInput, float verticalInput)
        {
            if (horizontalInput != 0 || verticalInput != 0)
            {
                _bobTimer += Time.deltaTime * bobFrequency;
                _currentAmplitude = Mathf.Lerp(_currentAmplitude, bobAmplitude, Time.deltaTime * bobDamping);

                float horizontalBob = Mathf.Cos(_bobTimer) * _currentAmplitude;
                float verticalBob = Mathf.Sin(_bobTimer * 2) * _currentAmplitude;

                Vector3 bobPosition = new Vector3(horizontalBob, verticalBob, 0);

                weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, _originalWeaponPosition + bobPosition + _currentRecoilOffset, Time.deltaTime * bobDamping);
            }
            else
            {
                _currentAmplitude = Mathf.Lerp(_currentAmplitude, 0, Time.deltaTime * bobDamping);
                _bobTimer += Time.deltaTime * bobFrequency;

                float horizontalBob = Mathf.Cos(_bobTimer) * _currentAmplitude;
                float verticalBob = Mathf.Sin(_bobTimer * 2) * _currentAmplitude;

                Vector3 bobPosition = new Vector3(horizontalBob, verticalBob, 0);

                weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, _originalWeaponPosition + bobPosition + _currentRecoilOffset, Time.deltaTime * bobDamping);
            }
        }

        public void ApplyWeaponSway(float mouseX, float mouseY)
        {
            Vector3 swayPosition = new Vector3(-mouseX, -mouseY, 0) * swayAmount;

            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, _originalWeaponPosition + swayPosition + _currentRecoilOffset, Time.deltaTime * swaySmoothness);
        }

        public void ApplyRecoil()
        {
            weaponHolder.DOKill();

            Sequence recoilSequence = DOTween.Sequence();

            recoilSequence.Append(weaponHolder.DOLocalMove(_originalWeaponPosition + recoilAmount, recoilSpeed)
                .SetEase(Ease.OutQuad));
            recoilSequence.Append(weaponHolder.DOLocalMove(_originalWeaponPosition, recoilReturnSpeed)
                .SetEase(Ease.InQuad));

            weaponHolder.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, fadeOut: false);

            recoilSequence.Play();
        }
    }
}