using Assets.Scripts.Components.Character;
using Assets.Scripts.EventBus;
using Assets.Scripts.WeaponSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Character.Player
{
    public class PlayerWeaponHandler : MonoBehaviour
    {
        [SerializeField] private List<GameObject> weaponPrefabs = new List<GameObject>();
        [SerializeField] private List<GameObject> throwablePrefabs = new List<GameObject>();

        [Header("Weapon Layers")]
        [SerializeField] private LayerMask aimLayer;
        [SerializeField] private LayerMask ignoreLayer;

        [Header("Weapon Settings")]
        [SerializeField] private Transform weaponHolderContainer;

        [SerializeField] private float switchCooldown = 0.2f;
        [SerializeField] private float defaultRayDistance = 100f;

        public WeaponBase Weapon => _weaponController.CurrentWeapon;

        private CharacterWeaponControllerComponent _weaponController;
        private CharacterThrowableControllerComponent _throwableController;
        private CharacterWeaponSwitcherComponent _weaponSwitcher;
        private PlayerRaycastProviderComponent _raycastProvider;

        private EventBinding<Events.ShootPressed> _shootPressed;
        private EventBinding<Events.ShootThrowablePressed> _shootThrowablePressed;
        private EventBinding<Events.ScrollInput> _scrollInput;

        private void Awake()
        {
            _raycastProvider = new PlayerRaycastProviderComponent(Camera.main, aimLayer, ignoreLayer, defaultRayDistance);
            _weaponController = new CharacterWeaponControllerComponent(weaponPrefabs, weaponHolderContainer);
            _throwableController = new CharacterThrowableControllerComponent(throwablePrefabs, weaponHolderContainer);
            _weaponSwitcher = new CharacterWeaponSwitcherComponent(_weaponController, switchCooldown);

            _weaponController.SpawnWeapon(0);
        }

        private void OnEnable()
        {
            _shootPressed = new EventBinding<Events.ShootPressed>(OnShootPressed);
            EventBus<Events.ShootPressed>.Register(_shootPressed);
            _shootThrowablePressed = new EventBinding<Events.ShootThrowablePressed>(OnShootThrowablePressed);
            EventBus<Events.ShootThrowablePressed>.Register(_shootThrowablePressed);
            _scrollInput = new EventBinding<Events.ScrollInput>(OnScrollInput);
            EventBus<Events.ScrollInput>.Register(_scrollInput);
        }

        private void OnDisable()
        {
            EventBus<Events.ShootPressed>.Unregister(_shootPressed);
            EventBus<Events.ShootThrowablePressed>.Unregister(_shootThrowablePressed);
            EventBus<Events.ScrollInput>.Unregister(_scrollInput);
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnShootPressed(Events.ShootPressed shootPressed)
        {
            HandleWeaponUsage();
        }

        private void OnShootThrowablePressed(Events.ShootThrowablePressed shootThrowablePressed)
        {
            HandleThrowableWeaponUsage();
        }

        private void OnScrollInput(Events.ScrollInput scrollInput)
        {
            HandleWeaponSwitching(scrollInput.Axis.y);
        }

        private void HandleWeaponUsage()
        {
            Vector3 targetPoint = _raycastProvider.GetTargetPoint();

            _weaponController.TryAttack(targetPoint);
        }

        private void HandleThrowableWeaponUsage()
        {
            Vector3 targetPoint = _raycastProvider.GetTargetPoint();

            _throwableController.TryThrow(targetPoint);
        }

        private void HandleWeaponSwitching(float scrollValue)
        {
            int direction = Mathf.RoundToInt(Mathf.Sign(scrollValue));

            _weaponSwitcher.SwitchWeapon(direction);
        }
    }
}