using Assets.Scripts.EventBus;
using Cinemachine;
using UnityEngine;

namespace Assets.Scripts.GameManagement
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera playerCamera;

        private CinemachineBasicMultiChannelPerlin _playerNoise;

        private EventBinding<Events.PlayerInitialized> _playerInitialized;

        private void Awake()
        {
            _playerNoise = playerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        private void OnEnable()
        {
            _playerInitialized = new EventBinding<Events.PlayerInitialized>(OnPlayerInitialized);
            EventBus<Events.PlayerInitialized>.Register(_playerInitialized);
        }

        private void OnDisable()
        {
            EventBus<Events.PlayerInitialized>.Unregister(_playerInitialized);
        }

        private void OnPlayerInitialized(Events.PlayerInitialized playerInitialized)
        {
            playerCamera.Follow = playerInitialized.CameraTarget;
            playerCamera.LookAt = playerInitialized.CameraTarget;
        }
    }
}