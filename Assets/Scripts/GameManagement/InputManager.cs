using Assets.Scripts.EventBus;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.GameManagement
{
    public class InputManager : MonoBehaviour
    {
        private PlayerControls _playerControls;

        private void Awake()
        {
            _playerControls = new PlayerControls();
        }

        private void OnEnable()
        {
            _playerControls.OnFeet.Enable();

            _playerControls.OnFeet.Move.performed += OnMovePerformed;
            _playerControls.OnFeet.Move.canceled += OnMoveCanceled;
            _playerControls.OnFeet.Look.performed += OnLookPerformed;
            _playerControls.OnFeet.Look.canceled += OnLookCanceled;
            _playerControls.OnFeet.ShootThrowable.performed += OnShootThrowable;
            _playerControls.OnFeet.WeaponSwitch.performed += OnWeaponSwitchPerformed;
        }

        private void OnDisable()
        {
            _playerControls.OnFeet.Move.performed -= OnMovePerformed;
            _playerControls.OnFeet.Move.canceled -= OnMoveCanceled;
            _playerControls.OnFeet.Look.performed -= OnLookPerformed;
            _playerControls.OnFeet.Look.canceled -= OnLookCanceled;
            _playerControls.OnFeet.ShootThrowable.performed -= OnShootThrowable;
            _playerControls.OnFeet.WeaponSwitch.performed -= OnWeaponSwitchPerformed;

            _playerControls.OnFeet.Disable();
        }

        private void Update()
        {
            if (_playerControls.OnFeet.Shoot.ReadValue<float>() > 0)
            {
                EventBus<Events.ShootPressed>.Raise(new Events.ShootPressed());
            }
        }

        private void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            EventBus<Events.MoveInput>.Raise(new Events.MoveInput
            {
                Axis = ctx.ReadValue<Vector2>()
            });
        }

        private void OnMoveCanceled(InputAction.CallbackContext ctx)
        {
            EventBus<Events.MoveInput>.Raise(new Events.MoveInput
            {
                Axis = Vector2.zero
            });
        }

        private void OnLookPerformed(InputAction.CallbackContext ctx)
        {
            EventBus<Events.LookInput>.Raise(new Events.LookInput
            {
                Axis = ctx.ReadValue<Vector2>()
            });
        }

        private void OnLookCanceled(InputAction.CallbackContext ctx)
        {
            EventBus<Events.LookInput>.Raise(new Events.LookInput
            {
                Axis = Vector2.zero
            });
        }

        private void OnShootThrowable(InputAction.CallbackContext ctx)
        {
            EventBus<Events.ShootThrowablePressed>.Raise(new Events.ShootThrowablePressed());
        }

        private void OnWeaponSwitchPerformed(InputAction.CallbackContext ctx)
        {
            EventBus<Events.ScrollInput>.Raise(new Events.ScrollInput
            {
                Axis = new Vector2(0, ctx.ReadValue<Vector2>().y)
            });
        }
    }
}