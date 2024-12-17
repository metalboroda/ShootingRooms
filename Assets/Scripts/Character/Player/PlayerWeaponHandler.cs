using Assets.Scripts.WeaponSystem;
using UnityEngine;

namespace Assets.Scripts.Character.Player
{
    public class PlayerWeaponHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask aimLayer;
        [SerializeField] private LayerMask ignoreLayer;
        [Space]
        [SerializeField] private WeaponBase weapon;

        [Header("Weapon Holder Settings")]
        [SerializeField] private GameObject weaponHolder;

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
            HandleWeaponUseInput();
        }

        private void HandleWeaponUseInput()
        {
            if (Input.GetMouseButton(0) && _mainCamera != null)
            {
                Ray ray = _mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, aimLayer))
                {
                    if (IsIgnoredLayer(hit.collider.gameObject.layer) == false)
                    {
                        weapon.TryAttack(hit.point);
                        return;
                    }
                }

                weapon.TryAttack(ray.GetPoint(_defaultRayDistance));
            }
        }

        private bool IsIgnoredLayer(int layer)
        {
            return (ignoreLayer.value & (1 << layer)) != 0;
        }
    }
}