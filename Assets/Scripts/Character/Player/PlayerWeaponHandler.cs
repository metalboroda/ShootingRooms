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
        [SerializeField] private WeaponBase weapon;

        [Header("Weapon Movement")]
        [SerializeField] private PlayerWeaponMovementHandler playerWeaponMovementHandler;

        private readonly float _defaultRayDistance = 100f;

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            HandleWeaponUsage();

            if (playerWeaponMovementHandler != null)
            {
                playerWeaponMovementHandler.ApplyWeaponBob(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                playerWeaponMovementHandler.ApplyWeaponSway(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
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
                        weapon.TryAttack(hit.point);

                        playerWeaponMovementHandler?.ApplyRecoil();

                        return;
                    }
                }

                weapon.TryAttack(ray.GetPoint(_defaultRayDistance));

                playerWeaponMovementHandler?.ApplyRecoil();
            }
        }

        private bool IsIgnoredLayer(int layer)
        {
            return (ignoreLayer.value & (1 << layer)) != 0;
        }
    }
}