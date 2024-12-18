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
        }
    }
}