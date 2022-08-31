using System;
using Characters.Player.Characters.Enemy;
using Cube.Picked;
using Cube.Picked.Spawner;
using Interactive;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.Enemy.Bloodhound
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BloodhoundNPCController : MonoBehaviour, ICollectHandler
    {
        private const float StoppingDistance = 0.5f;

        [SerializeField, HideInInspector]
        private NavMeshAgent _agent;

        private SpeedHandler _speedHandler;
        
        private CubeSpawnArea _spawnArea;

        private Vector3 _targetPosition;
        
        private void OnValidate()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public void Construct(CubeSpawnArea spawnArea)
        {
            _spawnArea = spawnArea;

            _speedHandler = new SpeedHandler(_agent, transform);

            GoToNextPosition();
        }

        private void Update()
        {
            _speedHandler.Tick();
            
            if (_agent.pathPending)
                return;
            
            if (_agent.remainingDistance < StoppingDistance) 
                GoToNextPosition();
        }
        
        private void GoToNextPosition()
        {
            _targetPosition = _spawnArea.RandomPosition;
            _agent.SetDestination(_targetPosition);
        }

        public void HandleCollecting()
        {
            Debug.Log("Agent raised the cube!");
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                
                Gizmos.DrawSphere(_targetPosition, 0.4f);
            }
        }
    }
}