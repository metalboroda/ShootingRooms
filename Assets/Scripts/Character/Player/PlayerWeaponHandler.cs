using Assets.Scripts.EventBus;
using Assets.Scripts.WeaponSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Character.Player
{
    public class PlayerWeaponHandler : MonoBehaviour
    {
        [SerializeField] private List<GameObject> weaponPrefabs = new List<GameObject>();

        [Header("Weapon Layers")]
        [SerializeField] private LayerMask aimLayer;
        [SerializeField] private LayerMask ignoreLayer;

        [Header("Weapon Settings")]
        [SerializeField] private GameObject weaponHolderContainer;

        public WeaponBase Weapon { get; private set; }

        private int _currentWeaponIndex = 0;
        private readonly float _weaponSwitchCooldown = 0.2f;
        private float _lastWeaponSwitchTime = 0f;
        private readonly float _defaultRayDistance = 100f;

        private Camera _mainCamera;
        private PlayerWeaponAnimationHandler _playerWeaponMovementHandler;

        private EventBinding<Events.ShootPressed> _shootPressed;
        private EventBinding<Events.ScrollInput> _scrollInput;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _playerWeaponMovementHandler = GetComponent<PlayerWeaponAnimationHandler>();

            SpawnWeapon(0);
        }

        private void OnEnable()
        {
            _shootPressed = new EventBinding<Events.ShootPressed>(OnShootPressed);
            EventBus<Events.ShootPressed>.Register(_shootPressed);
            _scrollInput = new EventBinding<Events.ScrollInput>(OnScrollInput);
            EventBus<Events.ScrollInput>.Register(_scrollInput);
        }

        private void OnDisable()
        {
            EventBus<Events.ShootPressed>.Unregister(_shootPressed);
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

        private void OnScrollInput(Events.ScrollInput scrollInput)
        {
            HandleWeaponSwitching(scrollInput.Axis.y);
        }

        private void HandleWeaponUsage()
        {
            if (_mainCamera == null) return;

            Ray ray = _mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, aimLayer))
            {
                if (IsIgnoredLayer(hit.collider.gameObject.layer) == false)
                {
                    Weapon.TryAttack(hit.point);

                    return;
                }
            }

            Weapon.TryAttack(ray.GetPoint(_defaultRayDistance));
        }

        private void HandleWeaponSwitching(float value)
        {
            if (Time.time - _lastWeaponSwitchTime < _weaponSwitchCooldown) return;

            if (value > 0f)
            {
                SwitchWeapon((_currentWeaponIndex + 1) % weaponPrefabs.Count);

                _lastWeaponSwitchTime = Time.time;
            }
            else if (value < 0f)
            {
                SwitchWeapon((_currentWeaponIndex - 1 + weaponPrefabs.Count) % weaponPrefabs.Count);

                _lastWeaponSwitchTime = Time.time;
            }
        }

        private void SpawnWeapon(int index)
        {
            if (index < 0 || index >= weaponPrefabs.Count) return;

            foreach (Transform child in weaponHolderContainer.transform)
            {
                Destroy(child.gameObject);
            }

            GameObject weaponInstance = Instantiate(weaponPrefabs[index], weaponHolderContainer.transform);

            Weapon = weaponInstance.GetComponent<WeaponBase>();

            _currentWeaponIndex = index;
        }

        private void SwitchWeapon(int newIndex)
        {
            if (newIndex != _currentWeaponIndex)
            {
                SpawnWeapon(newIndex);
            }
        }

        private bool IsIgnoredLayer(int layer)
        {
            return (ignoreLayer.value & (1 << layer)) != 0;
        }
    }
}