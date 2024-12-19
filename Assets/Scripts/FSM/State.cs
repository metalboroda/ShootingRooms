namespace Assets.Scripts.FSM
{
    public abstract class State : IState
    {
        protected object Context;

        public virtual void Init(object context)
        {
            Context = context;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
    }
}