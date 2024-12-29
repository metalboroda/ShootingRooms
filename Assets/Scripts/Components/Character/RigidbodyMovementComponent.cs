using UnityEngine;

namespace Components.Character
{
    public class RigidbodyMovementComponent
    {
        // Configuration fields
        private float _movementSpeed;
        private float _lookSensitivity;
        private LayerMask _groundLayer;
        private float _groundCheckDistance;
        private float _groundCheckRadius;
        private Vector3 _groundCheckOffset;

        // Dependencies
        private Rigidbody _rigidbody;
        private Transform _cameraTarget;

        // Internal variables
        private float _verticalLookRotation;

        public RigidbodyMovementComponent SetMovementSpeed(float movementSpeed)
        {
            _movementSpeed = movementSpeed;
            return this;
        }

        public RigidbodyMovementComponent SetLookSensitivity(float lookSensitivity)
        {
            _lookSensitivity = lookSensitivity;
            return this;
        }

        public RigidbodyMovementComponent SetGroundLayer(LayerMask groundLayer)
        {
            _groundLayer = groundLayer;
            return this;
        }

        public RigidbodyMovementComponent SetGroundCheckDistance(float groundCheckDistance)
        {
            _groundCheckDistance = groundCheckDistance;
            return this;
        }

        public RigidbodyMovementComponent SetGroundCheckRadius(float groundCheckRadius)
        {
            _groundCheckRadius = groundCheckRadius;
            return this;
        }

        public RigidbodyMovementComponent SetGroundCheckOffset(Vector3 groundCheckOffset)
        {
            _groundCheckOffset = groundCheckOffset;
            return this;
        }

        public RigidbodyMovementComponent SetRigidbody(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
            return this;
        }

        public RigidbodyMovementComponent SetCameraTarget(Transform cameraTarget)
        {
            _cameraTarget = cameraTarget;
            return this;
        }

        public void HandleMovement(Vector2 inputDirection)
        {
            if (_rigidbody == null) return;

            Vector3 moveDirection = _rigidbody.transform.forward * inputDirection.y +
                                    _rigidbody.transform.right * inputDirection.x;

            moveDirection *= _movementSpeed;
            moveDirection.y = _rigidbody.velocity.y;

            _rigidbody.velocity = moveDirection;
        }

        public void HandleLook(float mouseDeltaX, float mouseDeltaY)
        {
            if (_rigidbody == null || _cameraTarget == null) return;

            _rigidbody.transform.Rotate(Vector3.up, mouseDeltaX * _lookSensitivity);

            _verticalLookRotation -= mouseDeltaY * _lookSensitivity;
            _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, -90f, 90f);

            _cameraTarget.localEulerAngles = new Vector3(_verticalLookRotation, 0f, 0f);
        }

        public bool IsGrounded()
        {
            Vector3 groundCheckStart = _rigidbody.transform.position + _groundCheckOffset;
            Vector3 groundCheckEnd = groundCheckStart + Vector3.down * _groundCheckDistance;

            return Physics.CheckCapsule(
                groundCheckStart, groundCheckEnd, _groundCheckRadius, _groundLayer);
        }
    }
}