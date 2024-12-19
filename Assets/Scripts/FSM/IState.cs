namespace Assets.Scripts.FSM
{
    public interface IState
    {
        void Init(object context);
        void Enter();
        void Exit();
        void Update();
        void FixedUpdate();
    }
}