using Assets.Scripts.EventBus;
using Assets.Scripts.SOs.WeaponSystem;
using Components.Character;
using Components.WeaponSystem;
using EventBus;
using UnityEngine;

namespace Character.Player
{
    [RequireComponent(typeof(PlayerWeaponHandler))]
    public class PlayerWeaponAnimationHandler : MonoBehaviour
    {
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private float cameraRecoilAmountMultiplier = 250f;
        [SerializeField] private float cameraRecoilSpeedMultiplier = 0.3f;

        private WeaponAnimationComponent _weaponAnimationComponent;
        private CameraRecoilComponent _cameraRecoilComponent;
        private PlayerWeaponHandler _weaponHandler;
        private WeaponAnimationDataSO _weaponAnimationData;

        private Vector2 _moveDirection;
        private Vector2 _lookDirection;

        private EventBinding<Events.WeaponEquipped> _weaponEquipped;
        private EventBinding<Events.MoveInput> _moveInput;
        private EventBinding<Events.LookInput> _lookInput;
        private EventBinding<Events.WeaponUsed> _weaponUsed;

        private void Awake()
        {
            _weaponHandler = GetComponent<PlayerWeaponHandler>();
        }

        private void OnEnable()
        {
            _weaponEquipped = new EventBinding<Events.WeaponEquipped>(OnWeaponEquipped);
            EventBus<Events.WeaponEquipped>.Register(_weaponEquipped);
            _moveInput = new EventBinding<Events.MoveInput>(OnMoveInput);
            EventBus<Events.MoveInput>.Register(_moveInput);
            _lookInput = new EventBinding<Events.LookInput>(OnLookInput);
            EventBus<Events.LookInput>.Register(_lookInput);
            _weaponUsed = new EventBinding<Events.WeaponUsed>(OnWeaponUsed);
            EventBus<Events.WeaponUsed>.Register(_weaponUsed);
        }

        private void OnDisable()
        {
            EventBus<Events.WeaponEquipped>.Unregister(_weaponEquipped);
            EventBus<Events.MoveInput>.Unregister(_moveInput);
            EventBus<Events.LookInput>.Unregister(_lookInput);
            EventBus<Events.WeaponUsed>.Unregister(_weaponUsed);
        }

        private void Update()
        {
            if (_weaponAnimationData == null) return;

            _weaponAnimationComponent.ApplyWeaponBob(_moveDirection);
            _weaponAnimationComponent.ApplyWeaponSway(_lookDirection);
        }

        private void OnWeaponEquipped(Events.WeaponEquipped weaponEquipped)
        {
            if (weaponEquipped.CharacterID != transform.GetInstanceID()) return;

            _weaponAnimationData = weaponEquipped.Weapon.WeaponAnimationData;

            _weaponAnimationComponent = new WeaponAnimationComponent(weaponHolder, _weaponAnimationData);
            _cameraRecoilComponent =
                new CameraRecoilComponent(cameraTarget, cameraRecoilAmountMultiplier, cameraRecoilSpeedMultiplier);
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
            if (weaponUsed.WeaponID != _weaponHandler.Weapon.transform.GetInstanceID()) return;

            _weaponAnimationComponent.ApplyRecoil();
            _cameraRecoilComponent.ApplyRecoil(_weaponAnimationData.RecoilAmount, _weaponAnimationData.RecoilSpeed);
        }
    }
}