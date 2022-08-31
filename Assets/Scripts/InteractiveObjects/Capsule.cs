using Characters.Player;
using Interactive;
using UnityEngine;

namespace InteractiveObjects
{
    public class Capsule : MonoBehaviour, IInteractable, IPickable
    {
        public Rigidbody Rigidbody => _rigidbody;
        public Transform Transform => transform;
        public bool CanInteract => true;
        public Vector3 Position => transform.position;
        public Collider Collider => _collider;

        [SerializeField]
        private Collider _collider;

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
        
        public void Interact(object sender)
        {
            if(sender is IPickupHandler handler)
                handler.HandlePickup(this);
        }
    }
}