using Assets.Scripts.EventBus;
using UnityEngine;

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
        }

        private void OnDisable()
        {
            _playerControls.OnFeet.Disable();
        }

        private void Update()
        {
            EventBus<Events.MoveInput>.Raise(new Events.MoveInput
            {
                Axis = _playerControls.OnFeet.Move.ReadValue<Vector2>(),
            });

            EventBus<Events.LookInput>.Raise(new Events.LookInput
            {
                Axis = _playerControls.OnFeet.Look.ReadValue<Vector2>(),
            });

            EventBus<Events.ScrollInput>.Raise(new Events.ScrollInput
            {
                Axis = new Vector2(0, _playerControls.OnFeet.WeaponSwitch.ReadValue<Vector2>().y),
            });

            if (_playerControls.OnFeet.Shoot.ReadValue<float>() > 0)
            {
                EventBus<Events.ShootPressed>.Raise(new Events.ShootPressed());
            }
        }
    }
}