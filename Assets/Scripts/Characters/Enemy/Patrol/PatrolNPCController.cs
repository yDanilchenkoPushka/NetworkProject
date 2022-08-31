using Characters.Player.Characters.Enemy;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.Enemy.Patrol
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PatrolNPCController : MonoBehaviour
    {
        private const float StoppingDistance = 0.5f;

        [SerializeField]
        private LoopMode loopMode;
        
        [SerializeField, HideInInspector]
        private NavMeshAgent _agent;

        private IPath _path;
        
        private SpeedHandler _speedHandler;

        private void OnValidate()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public void Construct(IPath path)
        {
            _path = path;

            _speedHandler = new SpeedHandler(_agent, transform);
            
            transform.position = _path.NextPosition(loopMode);
        }

        private void Update()
        {
            _speedHandler.Tick();

            if (_agent.pathPending)
                return;
            
            if (_agent.remainingDistance < StoppingDistance) 
                GoToNextPosition();
        }

        private void GoToNextPosition() => 
            _agent.SetDestination(_path.NextPosition(loopMode));
    }
}