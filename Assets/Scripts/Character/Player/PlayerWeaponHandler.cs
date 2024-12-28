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
        [SerializeField] private GameObject weaponHolderContainer;

        public WeaponBase Weapon { get; private set; }
        public WeaponBase ThrowableWeapon { get; private set; }

        private int _currentWeaponIndex = 0;
        private int _currentThrowableWeaponIndex = 0;
        private readonly float _weaponSwitchCooldown = 0.2f;
        private float _lastWeaponSwitchTime = 0f;
        private readonly float _defaultRayDistance = 100f;

        private Camera _mainCamera;

        private EventBinding<Events.ShootPressed> _shootPressed;
        private EventBinding<Events.ShootThrowablePressed> _shootThrowablePressed;
        private EventBinding<Events.ScrollInput> _scrollInput;

        private void Awake()
        {
            _mainCamera = Camera.main;

            SpawnWeapon(0);
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
            if (_mainCamera == null) return;

            Vector3 targetPoint = GetRaycastHitPoint();

            Weapon.TryAttack(targetPoint);
        }

        private void HandleThrowableWeaponUsage()
        {
            if (_mainCamera == null) return;

            Vector3 targetPoint = GetRaycastHitPoint();

            SpawnAndUseThrowableWeapon(targetPoint);
        }

        private void SpawnAndUseThrowableWeapon(Vector3 targetPoint)
        {
            if (_currentThrowableWeaponIndex < 0 || _currentThrowableWeaponIndex >= throwablePrefabs.Count) return;

            GameObject weaponInstance = Instantiate(throwablePrefabs[_currentThrowableWeaponIndex], weaponHolderContainer.transform);

            ThrowableWeapon = weaponInstance.GetComponent<WeaponBase>();
            ThrowableWeapon.TryAttack(targetPoint);

            ThrowableWeapon = null;
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

        private void SwitchWeapon(int newIndex)
        {
            if (newIndex != _currentWeaponIndex)
            {
                SpawnWeapon(newIndex);
            }
        }

        private Vector3 GetRaycastHitPoint()
        {
            Ray ray = _mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, aimLayer))
            {
                if (IsIgnoredLayer(hit.collider.gameObject.layer) == false)
                {
                    return hit.point;
                }
            }

            return ray.GetPoint(_defaultRayDistance);
        }

        private bool IsIgnoredLayer(int layer)
        {
            return (ignoreLayer.value & (1 << layer)) != 0;
        }
    }
}