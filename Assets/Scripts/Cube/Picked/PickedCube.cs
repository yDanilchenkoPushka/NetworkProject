using System;
using Characters.Enemy.Patrol;
using Interactive;
using Score;
using UnityEngine;

namespace Cube.Picked
{
    public class PickedCube : MonoBehaviour, IBoundable
    {
        public event Action<PickedCube> OnPicked;
        public Vector3 Position => transform.position;
        public Bounds Bound => _boxCollider.bounds;
        
        public Vector3 Size => new Vector3(
            _boxCollider.size.x * transform.localScale.x, 
            _boxCollider.size.y * transform.localScale.y,
            _boxCollider.size.z * transform.localScale.z);
        
        [SerializeField]
        private BoxCollider _boxCollider;

        [SerializeField]
        private float _rotationSpeed;
        

        public void Initialize(Transform root)
        {
            gameObject.SetActive(false);
            
            transform.parent = root;
        }

        public void Spawn(Vector3 at, Transform root)
        {
            transform.parent = root;
            
            transform.position = at;
            transform.rotation = Quaternion.identity;
            
            gameObject.SetActive(true);
        }

        public void DeSpawn(Transform root)
        {
            transform.parent = root;
            
            gameObject.SetActive(false);
        }

        private void Update() => 
            Rotate();

        private void Rotate() => 
            transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ICollectHandler handler))
            {
                handler.HandleCollecting();
                
                OnPicked?.Invoke(this);
            }
        }
    }
}