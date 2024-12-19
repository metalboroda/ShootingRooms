using UnityEngine;

namespace Assets.Scripts.Components.Character
{
    public class CharacterControllerMovementComponent
    {
        // Configuration fields
        private float _movementSpeed;
        private float _lookSensitivity;
        private float _gravity;
        private LayerMask _groundLayer;
        private float _groundCheckDistance;
        private float _groundCheckRadius;
        private float _groundCheckRayDistance;
        private Vector3 _groundCheckOffset;
        private float _slipSpeed;
        private Vector3 _wallCheckOffset;

        // Ground Checking
        private Transform _groundCheckingPoint;

        // Internal variables
        private Vector3 _velocity;
        private float _verticalLookRotation;

        // Dependencies
        private CharacterController _characterController;
        private Transform _cameraTarget;

        // Setters (Builder Pattern)
        public CharacterControllerMovementComponent SetMovementSpeed(float movementSpeed)
        {
            _movementSpeed = movementSpeed;
            return this;
        }

        public CharacterControllerMovementComponent SetLookSensitivity(float lookSensitivity)
        {
            _lookSensitivity = lookSensitivity;
            return this;
        }

        public CharacterControllerMovementComponent SetGravity(float gravity)
        {
            _gravity = gravity;
            return this;
        }

        public CharacterControllerMovementComponent SetGroundLayer(LayerMask groundLayer)
        {
            _groundLayer = groundLayer;
            return this;
        }

        public CharacterControllerMovementComponent SetGroundCheckDistance(float groundCheckDistance)
        {
            _groundCheckDistance = groundCheckDistance;
            return this;
        }

        public CharacterControllerMovementComponent SetGroundCheckRadius(float groundCheckRadius)
        {
            _groundCheckRadius = groundCheckRadius;
            return this;
        }

        public CharacterControllerMovementComponent SetGroundCheckRayDistance(float groundCheckRayDistance)
        {
            _groundCheckRayDistance = groundCheckRayDistance;
            return this;
        }

        public CharacterControllerMovementComponent SetGroundCheckOffset(Vector3 groundCheckOffset)
        {
            _groundCheckOffset = groundCheckOffset;
            return this;
        }

        public CharacterControllerMovementComponent SetSlipSpeed(float slipSpeed)
        {
            _slipSpeed = slipSpeed;
            return this;
        }

        public CharacterControllerMovementComponent SetWallCheckOffset(Vector3 wallCheckOffset)
        {
            _wallCheckOffset = wallCheckOffset;
            return this;
        }

        public CharacterControllerMovementComponent SetGroundCheckingPoint(Transform groundCheckingPoint)
        {
            _groundCheckingPoint = groundCheckingPoint;
            return this;
        }

        public CharacterControllerMovementComponent SetCharacterController(CharacterController characterController)
        {
            _characterController = characterController;
            return this;
        }

        public CharacterControllerMovementComponent SetCameraTarget(Transform cameraTarget)
        {
            _cameraTarget = cameraTarget;
            return this;
        }

        // Public Methods

        /// <summary>
        /// Handles player movement based on input.
        /// </summary>
        public void HandleMovement(Vector2 inputDirection)
        {
            if (_characterController == null) return;

            Vector3 moveDirection = _characterController.transform.forward * inputDirection.y +
                                    _characterController.transform.right * inputDirection.x;

            moveDirection *= _movementSpeed;

            _characterController.Move(moveDirection * Time.deltaTime);
        }

        /// <summary>
        /// Handles player look rotation based on mouse input.
        /// </summary>
        public void HandleLook(float mouseDeltaX, float mouseDeltaY)
        {
            if (_characterController == null || _cameraTarget == null) return;

            _characterController.transform.Rotate(Vector3.up, mouseDeltaX * _lookSensitivity);

            _verticalLookRotation -= mouseDeltaY * _lookSensitivity;
            _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, -90f, 90f);

            _cameraTarget.localEulerAngles = new Vector3(_verticalLookRotation, 0f, 0f);
        }

        /// <summary>
        /// Applies gravity to the character.
        /// </summary>
        public void ApplyGravity()
        {
            if (_characterController == null || _characterController.isGrounded)
            {
                _velocity.y = 0f;
            }
            else
            {
                _velocity.y -= -_gravity * Time.deltaTime;
            }

            _characterController.Move(_velocity * Time.deltaTime);
        }

        /// <summary>
        /// Checks if the character is grounded using capsule check.
        /// </summary>
        public bool IsGrounded()
        {
            Vector3 groundCheckStart = _characterController.transform.position + _groundCheckOffset;
            Vector3 groundCheckEnd = groundCheckStart + Vector3.down * _groundCheckDistance;

            return Physics.CheckCapsule(
                groundCheckStart, groundCheckEnd, _groundCheckRadius, _groundLayer);
        }

        /// <summary>
        /// Checks if the character is grounded using raycast.
        /// </summary>
        public bool IsGroundedRay()
        {
            Vector3 groundCheckStart = _characterController.transform.position + _groundCheckOffset;

            return Physics.Raycast(
                groundCheckStart, Vector3.down, _groundCheckRayDistance, _groundLayer);
        }

        /// <summary>
        /// Checks for slip and applies movement to counteract it.
        /// </summary>
        public void ApplySlipChecking(bool isGrounded)
        {
            if (isGrounded == true) return;

            Vector3 raySpawnPos = _groundCheckingPoint.position + Vector3.up * _wallCheckOffset.y;

            // Directions
            Vector3 front = _groundCheckingPoint.forward * _wallCheckOffset.x;
            Vector3 back = -_groundCheckingPoint.forward * _wallCheckOffset.x;
            Vector3 left = -_groundCheckingPoint.right * _wallCheckOffset.x;
            Vector3 right = _groundCheckingPoint.right * _wallCheckOffset.x;

            // Combined directions
            Vector3 frontLeft = front + left;
            Vector3 frontRight = front + right;
            Vector3 backLeft = back + left;
            Vector3 backRight = back + right;

            // Rays
            Ray[] rays = new[]
            {
                new Ray(raySpawnPos, front), new Ray(raySpawnPos, back),
                new Ray(raySpawnPos, left), new Ray(raySpawnPos, right),
                new Ray(raySpawnPos, frontLeft), new Ray(raySpawnPos, frontRight),
                new Ray(raySpawnPos, backLeft), new Ray(raySpawnPos, backRight)
            };

            float dist = _wallCheckOffset.x;

            foreach (var ray in rays)
            {
                if (Physics.Raycast(ray, out RaycastHit hit, dist, _groundLayer))
                {
                    HitForSlip(hit.normal);
                    break;
                }
            }
        }

        // Private Methods

        /// <summary>
        /// Moves character in the direction of the slip.
        /// </summary>
        private void HitForSlip(Vector3 slipDirection)
        {
            _characterController.Move(((slipDirection * _slipSpeed) + Vector3.down) * Time.deltaTime);
        }
    }
}