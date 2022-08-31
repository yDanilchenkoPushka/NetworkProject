using System;
using Characters.Enemy.Following;
using Cube.Picked;
using Damage;
using Interactive;
using Score;
using Services.Input;
using UnityEngine;

namespace Characters.Player
{
    [RequireComponent(typeof(Rigidbody),
        typeof(Collider))]
    public class PlayerController : MonoBehaviour, IDamageable, IScoreWriter, IScoreReader,
        IPickupHandler, ILookable, IPositionable, IEnemyTarget, IOut<IInteractionEvents>, ICollectHandler
    {
        IInteractionEvents IOut<IInteractionEvents>.Value => _playerInteraction;
        
        public event Action OnDamaged;
        public event Action<int> OnScoreUpdated;

        public Vector3 Position => transform.position;
        public Vector3 LookDirection => _lookDirection;

        [SerializeField]
        private PlayerMovement _playerMovement;

        [SerializeField]
        private PlayerPickup _playerPickup;

        [SerializeField]
        private CheckerTrigger _interactionTrigger;
        
        [SerializeField, HideInInspector]
        private Rigidbody _rigidbody;

        [SerializeField, HideInInspector]
        private Collider _collider;

        private ISimpleInput _simpleInput;
        private int _currentScore;

        private PlayerAgent _playerAgent;
        private PlayerInteraction _playerInteraction;
        
        private Vector3 _lookDirection;
        private PlayerInteraction _value;

        private void OnValidate()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        public void Construct(ISimpleInput simpleInput)
        {
            _simpleInput = simpleInput;

            _playerAgent = new PlayerAgent();
            _playerAgent.Construct(transform);
            
            _playerInteraction = new PlayerInteraction(_interactionTrigger, _simpleInput, this);
        
            _playerMovement.Construct(_rigidbody, simpleInput, _playerAgent);
            _playerPickup.Construct(transform, _rigidbody, _collider, this);

            _lookDirection = transform.forward;
        }

        private void OnDestroy() => 
            DeInitialize();

        private void DeInitialize()
        {
            _playerInteraction.DeInitialize();
            
            _simpleInput = null;
        }

        private void Update()
        {
            _playerAgent.Tick();
            UpdateLook();
            _playerPickup.Tick();
        }

        private void FixedUpdate() => 
            _playerMovement.FixedTick();

        public void Spawn(Vector3 at)
        {
            transform.position = at;
            transform.rotation = Quaternion.identity;
        }

        public void TakeDamage() => 
            OnDamaged?.Invoke();

        public void Accrue(int score)
        {
            _currentScore += score;
        
            OnScoreUpdated?.Invoke(_currentScore);
        }

        public void HandlePickup(IPickable pickable) => 
            _playerPickup.HandlePickup(pickable);

        private void UpdateLook()
        {
            Vector2 look = _simpleInput.LookAxis;
        
            if(look.magnitude >= 0.1f)
                _lookDirection = new Vector3(look.x, 0, look.y).normalized;
        }

        public void HandleCollecting()
        {
            Accrue(1);
        }
    }
}