namespace Assets.Scripts.FSM
{
    public interface IState
    {
        public void Init(object context);
        public void Enter();
        public void Exit();
        public void Update();
        public void FixedUpdate();
        public void LateUpdate();
    }
}