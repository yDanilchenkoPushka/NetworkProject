using Characters.Player.Characters.Enemy;
using Infrastructure;
using Infrastructure.Processors.Tick;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.Enemy.Following.States
{
    public class FollowingState : IState, ITickable
    {
        private readonly IStateMachine _stateMachine;
        private readonly Transform _transform;
        private readonly NavMeshAgent _agent;

        private readonly ITargetInfo _targetInfo;
        private readonly float _minDistance;
        private readonly ITickProcessor _tickProcessor;
        private readonly SpeedHandler _speedHandler;

        public FollowingState(IStateMachine stateMachine, Transform transform, NavMeshAgent agent, float minDistance, 
            ITargetInfo targetInfo, ITickProcessor tickProcessor, SpeedHandler speedHandler)
        {
            _stateMachine = stateMachine;
            _transform = transform;
            _agent = agent;
            _minDistance = minDistance;
            _targetInfo = targetInfo;
            _tickProcessor = tickProcessor;
            _speedHandler = speedHandler;
        }

        public void Enter()
        {
            _tickProcessor.Add(this);
            
            _targetInfo.OnTargetUpdated += ToNextState;
        }

        public void Exit()
        {
            _tickProcessor.Remove(this);
            
            _targetInfo.OnTargetUpdated -= ToNextState;
        }

        public void Tick()
        {
            _speedHandler.Tick();
            Follow();
        }

        private void Follow()
        {
            Vector3 targetPosition = _targetInfo.Target.Position;
            Vector3 direction = (targetPosition - _transform.position).normalized;
            
            float distance = Vector3.Distance(targetPosition, _transform.position);
            
            if (distance < _minDistance)
                return;
            
            Vector3 position = _transform.position + direction * _minDistance;
            
            _agent.SetDestination(position);
        }
        
        private void ToNextState()
        {
            if(_targetInfo.HasTarget)
                return;
            
            _stateMachine.ChangeState<IdleState>();
        }
    }
}