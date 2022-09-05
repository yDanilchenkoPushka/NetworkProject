using System;
using Characters.Player;
using Interactive;
using Services;
using Unity.Netcode;
using UnityEngine;

namespace InteractiveObjects
{
    public class Capsule : NetworkBehaviour, IInteractable, IPickable
    {
        public Rigidbody Rigidbody => _rigidbody;
        public Transform Transform => transform;
        public ulong ObjectId => _networkObject.NetworkObjectId;
        public bool IsDirty => _isDirty;
        public bool CanInteract => true;
        public Vector3 Position => transform.position;
        public Collider Collider => _collider;

        [SerializeField]
        private Collider _collider;

        [SerializeField, HideInInspector]
        private Rigidbody _rigidbody;

        [SerializeField, HideInInspector]
        private NetworkObject _networkObject;
        
        private bool _isDirty;

        private void OnValidate()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _networkObject = GetComponent<NetworkObject>();
        }

        public void EnterInteractive()
        {
            _isDirty = true;
        }

        public void ExitInteractive()
        {
            _isDirty = false;
        }
        
        public void Interact(object sender)
        {
            if(sender is IPickupHandler handler)
                handler.HandlePickup(this);
        }
    }
}