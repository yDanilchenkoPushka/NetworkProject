using System;
using UnityEngine;

namespace Characters.Player
{
    [Serializable]
    public class PlayerPickup
    {
        public bool IsHeld => _pickable != null;

        [SerializeField]
        private float _pickupForce = 150f;

        [SerializeField]
        private float _holdDistance = 1.2f;
        
        private IPickable _pickable;
        private Transform _transform;
        private Rigidbody _rigidbody;
        private Collider _collider;
        private ILookable _lookable;

        public void Construct(Transform transform, Rigidbody rigidbody, Collider collider, ILookable lookable)
        {
            _transform = transform;
            _rigidbody = rigidbody;
            _collider = collider;
            _lookable = lookable;
        }

        public void Tick()
        {
            if(IsHeld)
                MoveObject();
        }

        public void HandlePickup(IPickable pickable)
        {
            if(IsHeld)
                DropObject();
            else
                PickupObject(pickable);
        }

        private void MoveObject()
        {
            Vector3 forwardPosition = GetForwardPosition();
            
            if (Vector3.Distance(_pickable.Position, GetForwardPosition()) > 0.1f)
            {
                Vector3 moveDirection = forwardPosition - _pickable.Position;
                _pickable.Rigidbody.AddForce(moveDirection * _pickupForce);
            }
        }

        private void PickupObject(IPickable pickable)
        {
            _pickable = pickable;

            Rigidbody rigidbody = pickable.Rigidbody;
            rigidbody.useGravity = false;
            rigidbody.drag = 10;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            rigidbody.position = GetForwardPosition();
            
            Physics.IgnoreCollision(_collider, pickable.Collider, true);
        }

        private void DropObject()
        {
            Rigidbody rigidbody = _pickable.Rigidbody;
            rigidbody.useGravity = true;
            rigidbody.drag = 0;
            rigidbody.constraints = RigidbodyConstraints.None;

            Physics.IgnoreCollision(_collider, _pickable.Collider, false);
            
            _pickable.Transform.parent = null;

            _pickable = null;
        }

        private Vector3 GetForwardPosition() => 
            _transform.position + _lookable.LookDirection * _holdDistance;
    }
}