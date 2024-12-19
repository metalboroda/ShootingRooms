using Assets.Scripts.EventBus;
using Assets.Scripts.SOs.WeaponSystem;
using DG.Tweening;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace Assets.Scripts.Character.Player
{
    [RequireComponent(typeof(PlayerWeaponHandler))]
    public class PlayerWeaponAnimationHandler : MonoBehaviour
    {
        [SerializeField] private Transform weaponHolder;

        private Vector2 _moveDirection;
        private Vector2 _lookDirection;
        private Vector3 _originalWeaponPosition;
        private float _bobTimer;
        private float _currentAmplitude;
        private Vector3 _currentRecoilOffset;

        private const float BobSinMultiplier = 2f;
        private const float IdleBobDampingDivider = 4f;

        private PlayerWeaponHandler _weaponHandler;
        private WeaponDataSO _weaponData;
        private WeaponAnimationDataSO _weaponAnimationData;

        private EventBinding<Events.MoveInput> _moveInput;
        private EventBinding<Events.LookInput> _lookInput;
        private EventBinding<Events.WeaponUsed> _weaponUsed;

        private void Awake()
        {
            _weaponHandler = GetComponent<PlayerWeaponHandler>();

            if (weaponHolder != null)
            {
                _originalWeaponPosition = weaponHolder.localPosition;
            }
        }

        private void OnEnable()
        {
            _moveInput = new EventBinding<Events.MoveInput>(OnMoveInput);
            EventBus<Events.MoveInput>.Register(_moveInput);
            _lookInput = new EventBinding<Events.LookInput>(OnLookInput);
            EventBus<Events.LookInput>.Register(_lookInput);
            _weaponUsed = new EventBinding<Events.WeaponUsed>(OnWeaponUsed);
            EventBus<Events.WeaponUsed>.Register(_weaponUsed);
        }

        private void OnDisable()
        {
            EventBus<Events.MoveInput>.Unregister(_moveInput);
            EventBus<Events.LookInput>.Unregister(_lookInput);
            EventBus<Events.WeaponUsed>.Unregister(_weaponUsed);
        }

        private void Start()
        {
            _weaponData = _weaponHandler.Weapon.WeaponData;
            _weaponAnimationData = _weaponData.WeaponAnimationData;
        }

        private void Update()
        {
            ApplyWeaponBob(_moveDirection.x, _moveDirection.y);
            ApplyWeaponSway(_lookDirection.x, _lookDirection.y);
        }

        private void OnMoveInput(Events.MoveInput moveInput)
        {
            _moveDirection = moveInput.Axis;
        }

        private void OnLookInput(Events.LookInput lookInput)
        {
            _lookDirection = lookInput.Axis;
        }

        private void OnWeaponUsed(Events.WeaponUsed weaponUsed)
        {
            if (weaponUsed.ID != _weaponHandler.Weapon.transform.GetInstanceID()) return;

            ApplyRecoil();
        }

        public void ApplyWeaponBob(float horizontalInput, float verticalInput)
        {
            if (horizontalInput != 0 || verticalInput != 0)
            {
                _bobTimer += Time.deltaTime * _weaponAnimationData.BobFrequency;
                _currentAmplitude = Mathf.Lerp(
                    _currentAmplitude, _weaponAnimationData.BobAmplitude, Time.deltaTime * _weaponAnimationData.BobDamping);

                float horizontalBob = Mathf.Cos(_bobTimer) * _currentAmplitude;
                float verticalBob = Mathf.Sin(_bobTimer * BobSinMultiplier) * _currentAmplitude;

                Vector3 bobPosition = new Vector3(horizontalBob, verticalBob, 0);

                weaponHolder.localPosition = Vector3.Lerp(
                    weaponHolder.localPosition, _originalWeaponPosition + bobPosition + _currentRecoilOffset, Time.deltaTime * _weaponAnimationData.BobDamping);
            }
            else
            {
                _currentAmplitude = Mathf.Lerp(_currentAmplitude, 0, Time.deltaTime * _weaponAnimationData.BobDamping);
                _bobTimer += Time.deltaTime * _weaponAnimationData.BobFrequency;

                float horizontalBob = Mathf.Cos(_bobTimer) * _currentAmplitude;
                float verticalBob = Mathf.Sin(_bobTimer / IdleBobDampingDivider) * _currentAmplitude;

                Vector3 bobPosition = new Vector3(horizontalBob, verticalBob, 0);

                weaponHolder.localPosition = Vector3.Lerp(
                    weaponHolder.localPosition, _originalWeaponPosition + bobPosition + _currentRecoilOffset, Time.deltaTime * (_weaponAnimationData.BobDamping / IdleBobDampingDivider));
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