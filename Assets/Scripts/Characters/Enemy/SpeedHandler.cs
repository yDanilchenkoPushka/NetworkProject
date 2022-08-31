using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Characters.Player.Characters.Enemy
{
    public class SpeedHandler
    {
        private readonly NavMeshAgent _agent;
        private readonly Transform _transform;
        
        private readonly float _startSpeed;

        public SpeedHandler(NavMeshAgent agent, Transform transform)
        {
            _agent = agent;
            _transform = transform;

            _startSpeed = agent.speed;
        }

        public void Tick()
        {
            Vector3 position = _transform.position;

            position.y -= 1f;

            if (NavMeshUtils.TryGetCoast(position, out float coast))
            {
                _agent.speed = (coast < 0)
                    ? _startSpeed
                    : _startSpeed / coast;
            }
        }
    }
}