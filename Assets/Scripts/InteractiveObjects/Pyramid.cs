using Interactive;
using UnityEngine;

namespace InteractiveObjects
{
    [RequireComponent(typeof(Rigidbody))]
    public class Pyramid : MonoBehaviour, IInteractable
    {
        public bool CanInteract => true;
        public Vector3 Position => transform.position;
        
        [SerializeField]
        private float _force;

        [SerializeField, HideInInspector]
        private Rigidbody _rigidbody;

        private void OnValidate() => 
            _rigidbody = GetComponent<Rigidbody>();

        public void EnterInteractive()
        {
        }

        public void ExitInteractive()
        {
        }
        
        public void Interact(object sender) => 
            Jump();

        private void Jump() => 
            _rigidbody.AddForce(Vector3.up * _force, ForceMode.Force);
    }
}