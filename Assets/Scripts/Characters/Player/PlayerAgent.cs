using System;
using UnityEngine;
using Utilities;

namespace Characters.Player
{
    public class PlayerAgent
    {
        public event Action<float> OnAreaChanged;
        
        private Transform _transform;

        public void Construct(Transform transform) => 
            _transform = transform;

        public void Tick() => 
            CheckArea();

        private void CheckArea()
        {
            Vector3 position = _transform.position;

            position.y -= 0.5f;

            if (NavMeshUtils.TryGetCoast(position, out float coast)) 
                OnAreaChanged?.Invoke(coast);
        }
    }
}