using System;
using Services.Input;
using Unity.Netcode;
using UnityEngine;

namespace Characters.Player
{
    [Serializable]
    public class PlayerMovement
    {
        [SerializeField]
        private float _speed = 14f;
        
        private Rigidbody _rigidbody;
        private PlayerAgent _agent;

        public void Construct(Rigidbody rigidbody, PlayerAgent agent)
        {
            _rigidbody = rigidbody;
            _agent = agent;

            _agent.OnAreaChanged += OnAreaChanged;
        }

        public void Move(Vector2 direction)
        {
            Vector3 movement = new Vector3(direction.x * _speed, 0, direction.y * _speed);
            
            _rigidbody.AddForce(movement, ForceMode.Acceleration);
        }
        
        private void OnAreaChanged(float coast)
        {
            float drag = (coast < 0)
                ? 0
                : (coast - 1f) * 5f;
            
            _rigidbody.drag = drag;
        }
    }
}