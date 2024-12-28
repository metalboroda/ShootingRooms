using Assets.Scripts.Components.Character;
using Assets.Scripts.EventBus;
using UnityEngine;

namespace Assets.Scripts.Character.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementHandler : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] public float movementSpeed;
        [SerializeField] public float lookSensitivity;

        [Header("Gravity")]
        [SerializeField] public float gravity = -9.81f;

        [Header("Ground Checking")]
        [SerializeField] public LayerMask groundLayer;
        [SerializeField] public float groundCheckDistance = 0.5f;
        [SerializeField] public float groundCheckRadius = 0.249f;

        [Header("Ground Checking Ray")]
        [SerializeField] public float groundCheckRayDistance = 0.5f;
        [SerializeField] public Vector3 groundCheckOffset = Vector3.zero;

        [Header("Slip Checking")]
        [SerializeField] public float slipSpeed;
        [SerializeField] public Vector3 wallCheckOffset;

        [Header("References")]
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private Transform groundCheckingPoint;

        public Vector2 MoveDirection { get; private set; }
        public Vector2 LookDirection { get; private set; }

        public CharacterControllerMovementComponent CharacterControllerMovementComponent { get; private set; }

        private CharacterController _characterController;

        private PlayerControls _playerControls;

        private EventBinding<Events.MoveInput> _moveInput;
        private EventBinding<Events.LookInput> _lookInput;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();

            _playerControls = new PlayerControls();

            CharacterControllerMovementComponent = new CharacterControllerMovementComponent()
             .SetCharacterController(_characterController)
             .SetCameraTarget(cameraTarget)
             .SetGroundCheckingPoint(groundCheckingPoint)
             .SetMovementSpeed(movementSpeed)
             .SetLookSensitivity(lookSensitivity)
             .SetGravity(gravity)
             .SetGroundLayer(groundLayer)
             .SetGroundCheckDistance(groundCheckDistance)
             .SetGroundCheckRadius(groundCheckRadius)
             .SetGroundCheckRayDistance(groundCheckRayDistance)
             .SetGroundCheckOffset(groundCheckOffset)
             .SetSlipSpeed(slipSpeed)
             .SetWallCheckOffset(wallCheckOffset);

            _playerControls.OnFeet.Enable();
        }

        private void OnEnable()
        {
            _moveInput = new EventBinding<Events.MoveInput>(OnMoveInput);
            EventBus<Events.MoveInput>.Register(_moveInput);
            _lookInput = new EventBinding<Events.LookInput>(OnLookInput);
            EventBus<Events.LookInput>.Register(_lookInput);
        }

        private void OnDisable()
        {
            EventBus<Events.MoveInput>.Unregister(_moveInput);
            EventBus<Events.LookInput>.Unregister(_lookInput);
        }

        private void Start()
        {
            EventBus<Events.PlayerInitialized>.Raise(new Events.PlayerInitialized
            {
                CameraTarget = cameraTarget,
            });
        }

        private void OnDestroy()
        {
            _playerControls.OnFeet.Disable();
        }

        private void OnMoveInput(Events.MoveInput moveInput)
        {
            MoveDirection = moveInput.Axis;
        }

        private void OnLookInput(Events.LookInput lookInput)
        {
            LookDirection = lookInput.Axis;
        }

        private void OnDrawGizmosSelected()
        {
            // Ground Checking
            Gizmos.color = Color.red;

            Vector3 groundCheckStart = transform.position + groundCheckOffset;
            Vector3 groundCheckEnd = groundCheckStart + Vector3.down * groundCheckDistance;

            Gizmos.DrawWireSphere(groundCheckStart, groundCheckRadius);
            Gizmos.DrawWireSphere(groundCheckEnd, groundCheckRadius);

            // Grounded Ray
            Gizmos.DrawRay(groundCheckStart, Vector3.down * groundCheckRayDistance);

            // Slip Checking
            Vector3 _slipRayPosition = groundCheckingPoint.position + Vector3.up * wallCheckOffset.y;

            Vector3 front = groundCheckingPoint.forward * wallCheckOffset.x;
            Vector3 back = -groundCheckingPoint.forward * wallCheckOffset.x;
            Vector3 left = -groundCheckingPoint.right * wallCheckOffset.x;
            Vector3 right = groundCheckingPoint.right * wallCheckOffset.x;

            Vector3 frontLeft = front + left;
            Vector3 frontRight = front + right;
            Vector3 backLeft = back + left;
            Vector3 backRight = back + right;

            Gizmos.color = Color.blue;

            Gizmos.DrawRay(_slipRayPosition, front);
            Gizmos.DrawRay(_slipRayPosition, back);
            Gizmos.DrawRay(_slipRayPosition, left);
            Gizmos.DrawRay(_slipRayPosition, right);

            Gizmos.color = Color.red;

            Gizmos.DrawRay(_slipRayPosition, frontLeft);
            Gizmos.DrawRay(_slipRayPosition, frontRight);
            Gizmos.DrawRay(_slipRayPosition, backLeft);
            Gizmos.DrawRay(_slipRayPosition, backRight);
        }
    }
}