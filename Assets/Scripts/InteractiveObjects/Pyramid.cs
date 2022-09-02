using Interactive;
using Unity.Netcode;
using UnityEngine;

namespace InteractiveObjects
{
    [RequireComponent(typeof(Rigidbody))]
    public class Pyramid : NetworkBehaviour, IInteractable
    {
        public bool HasBusy => _hasBusy;
        public bool CanInteract => true;
        public Vector3 Position => transform.position;
        
        [SerializeField]
        private float _force;

        [SerializeField, HideInInspector]
        private Rigidbody _rigidbody;

        private bool _hasBusy;

        private void OnValidate() => 
            _rigidbody = GetComponent<Rigidbody>();

        public void EnterInteractive()
        {
            _hasBusy = true;
        }

        public void ExitInteractive()
        {
            _hasBusy = false;
        }
        
        public void Interact(object sender) => 
            Jump();

        private void Jump() => 
            _rigidbody.AddForce(Vector3.up * _force, ForceMode.Force);
    }
}