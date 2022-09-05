using Characters.Player;
using Interactive;
using Services;
using Unity.Netcode;
using UnityEngine;

namespace InteractiveObjects
{
    [RequireComponent(typeof(Rigidbody))]
    public class Pyramid : NetworkBehaviour, IInteractable
    {
        public bool IsDirty => _isDirty;
        public bool CanInteract => true;
        public Vector3 Position => transform.position;

        [SerializeField]
        private float _force;

        [SerializeField, HideInInspector]
        private Rigidbody _rigidbody;

        private bool _isDirty;

        private void OnValidate() => 
            _rigidbody = GetComponent<Rigidbody>();

        public void EnterInteractive()
        {
            _isDirty = true;
        }

        public void ExitInteractive()
        {
            _isDirty = false;
        }
        
        public void Interact(object sender) => 
            Jump();

        private void Jump() => 
            _rigidbody.AddForce(Vector3.up * _force, ForceMode.Force);
    }
}