using Characters.Player;
using Interactive;
using Unity.Netcode;
using UnityEngine;

namespace InteractiveObjects
{
    public class Capsule : NetworkBehaviour, IInteractable, IPickable
    {
        public Rigidbody Rigidbody => _rigidbody;
        public Transform Transform => transform;
        public bool HasBusy => _hasBusy;
        public bool CanInteract => true;
        public Vector3 Position => transform.position;
        public Collider Collider => _collider;

        [SerializeField]
        private Collider _collider;

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
        
        public void Interact(object sender)
        {
            if(sender is IPickupHandler handler)
                handler.HandlePickup(this);
        }
    }
}