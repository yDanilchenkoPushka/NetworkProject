namespace Infrastructure
{
    public interface IStateMachine
    {
        void ChangeState<TState>() where TState : IState;
    }
}