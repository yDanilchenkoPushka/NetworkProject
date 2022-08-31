using Infrastructure;

namespace Characters.Enemy.Following.States
{
    public class IdleState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly ITargetInfo _targetInfo;

        public IdleState(IStateMachine stateMachine, ITargetInfo targetInfo)
        {
            _stateMachine = stateMachine;
            _targetInfo = targetInfo;
        }

        public void Enter()
        {
            _targetInfo.OnTargetUpdated += ToNextState;
        }

        public void Exit()
        {
            _targetInfo.OnTargetUpdated -= ToNextState;
        }

        private void ToNextState()
        {
            if(_targetInfo.HasTarget)
                _stateMachine.ChangeState<FollowingState>();
        }
    }
}