namespace Assets.Scripts.Character.Player.States
{
    public class PlayerMovementState : PlayerBaseState
    {
        public override void Update()
        {
            CharacterControllerMovementComponent.HandleMovement(PlayerMovementHandler.MoveDirection);
            CharacterControllerMovementComponent.HandleLook(PlayerMovementHandler.LookDirection.x, PlayerMovementHandler.LookDirection.y);
            CharacterControllerMovementComponent.ApplyGravity();
            CharacterControllerMovementComponent.ApplySlipChecking(CharacterControllerMovementComponent.IsGrounded());
        }
    }
}