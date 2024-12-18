using Assets.Scripts.WeaponSystem;
using UnityEngine;

namespace Assets.Scripts.Character.Player
{
    public class PlayerWeaponHandler : MonoBehaviour
    {
        [Header("Weapon Layers")]
        [SerializeField] private LayerMask aimLayer;
        [SerializeField] private LayerMask ignoreLayer;

        [Header("Weapon Settings")]
        [SerializeField] private GameObject weaponHolderContainer;

        public WeaponBase Weapon { get; private set; }

        private readonly float _defaultRayDistance = 100f;
        private float _nextRecoilTime = 0f;

        private Camera _mainCamera;
        private PlayerWeaponMovementHandler _playerWeaponMovementHandler;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _playerWeaponMovementHandler = GetComponent<PlayerWeaponMovementHandler>();
            Weapon = weaponHolderContainer.GetComponentInChildren<WeaponBase>();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            HandleWeaponUsage();

            if (_playerWeaponMovementHandler != null)
            {
                _playerWeaponMovementHandler.ApplyWeaponBob(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                _playerWeaponMovementHandler.ApplyWeaponSway(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }
        }

        private void HandleWeaponUsage()
        {
            if (Input.GetMouseButton(0) && _mainCamera != null)
            {
                Ray ray = _mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, aimLayer))
                {
                    if (IsIgnoredLayer(hit.collider.gameObject.layer) == false)
                    {
                        Weapon.TryAttack(hit.point);

                        TryApplyRecoil();
                        return;
                    }
                }

                Weapon.TryAttack(ray.GetPoint(_defaultRayDistance));

                TryApplyRecoil();
            }
        }

        private void TryApplyRecoil()
        {
            if (Time.time >= _nextRecoilTime)
            {
                _playerWeaponMovementHandler?.ApplyRecoil();

                _nextRecoilTime = Time.time + Weapon.WeaponData.AttackRate;
            }
        }

        private bool IsIgnoredLayer(int layer)
        {
            return (ignoreLayer.value & (1 << layer)) != 0;
        }
    }
}