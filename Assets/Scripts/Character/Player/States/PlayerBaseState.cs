using Assets.Scripts.Components.Character;
using Assets.Scripts.FSM;

namespace Assets.Scripts.Character.Player.States
{
    public abstract class PlayerBaseState : State
    {
        protected PlayerController PlayerController;
        protected PlayerMovementHandler PlayerMovementHandler;

        protected RigidbodyMovementComponent RigidbodyMovementComponent;

        public override void Init(object context)
        {
            PlayerController = context as PlayerController;
            PlayerMovementHandler = PlayerController.PlayerMovmentHandler;

            RigidbodyMovementComponent = PlayerMovementHandler.RigidbodyMovementComponent;
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
        }

        public override void FixedUpdate()
        {
        }
    }
}