using System;
using Characters.Enemy.Following;
using Cube.Picked;
using Damage;
using Interactive;
using Score;
using Services;
using Services.Input;
using Unity.Netcode;
using UnityEngine;

namespace Characters.Player
{
    [RequireComponent(typeof(Rigidbody),
        typeof(Collider))]
    public class PlayerController : NetworkBehaviour, IDamageable, IScoreWriter, IScoreReader,
        IPickupHandler, ILookable, IPositionable, IEnemyTarget, IOut<IInteractionEvents>, ICollectHandler
    {
        IInteractionEvents IOut<IInteractionEvents>.Value => _playerInteraction;
        
        public event Action OnDamaged;
        public event Action<int> OnScoreUpdated;

        public Vector3 Position => transform.position;
        public Vector3 LookDirection => _lookDirection;

        [SerializeField] 
        private MeshRenderer _meshRenderer;
        
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

        [SerializeField]
        private Material[] _materials;

        private ISimpleInput _simpleInput;

        private PlayerAgent _playerAgent;
        private PlayerInteraction _playerInteraction;
        
        private PlayerInteraction _value;

        private bool _isConstructed;

        //private Vector3 _movementAxis;
        private ulong _localClientId;
        public static event Action<PlayerController> OnPlayerSpawned;
        public static event Action<PlayerController> OnPlayerDeSpawned;

        private Vector3 _movementAxis; 
        private Vector3 _lookDirection;

        private NetworkVariable<int> _currentScore = new NetworkVariable<int>();

        private void OnValidate()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            //_movementAxis.Value = Vector3.zero;
            _simpleInput = AllServices.Container.Single<ISimpleInput>();

            _playerAgent = new PlayerAgent();
            _playerAgent.Construct(transform);
            
            _playerInteraction = new PlayerInteraction(_interactionTrigger, _simpleInput, this);
        
            _playerMovement.Construct(_rigidbody, _playerAgent);
            _playerPickup.Construct(transform, _rigidbody, _collider, this);

            _lookDirection = transform.forward;

            //_meshRenderer.material = _materials[materialIndex];

            OnPlayerSpawned?.Invoke(this);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            OnPlayerDeSpawned?.Invoke(this);
        }

        private void OnDestroy() => 
            DeInitialize();

        private void DeInitialize()
        {
            if (!_isConstructed)
                return;
            
            _playerInteraction.DeInitialize();
            
            _simpleInput = null;
        }

        public void Tick()
        {
            // _playerAgent.Tick();
            // UpdateLook();
            // _playerPickup.Tick();
        }

        public void FixedTick()
        {
            _playerMovement.Move(_movementAxis);
        }

        private void Update()
        {
            if (!IsOwner)
                return;
            
            UpdateLookOnServerRpc(GetLook());
            UpdateMovementAxisOnServerRpc(_simpleInput.MovementAxis);
        }

        // public void Spawn(Vector3 at)
        // {
        //     transform.position = at;
        //     transform.rotation = Quaternion.identity;
        // }
        
        [ClientRpc]
        public void SpawnClientRpc(ulong localClientId, Vector3 at)
        {
            _localClientId = localClientId;
            
            transform.position = at;
            transform.rotation = Quaternion.identity;
        }

        public void TakeDamage() => 
            OnDamaged?.Invoke();

        public void Accrue(int score)
        {
            _currentScore.Value += score;
        
            OnScoreUpdated?.Invoke(_currentScore.Value);
            
            Debug.Log($"Updated score: {_currentScore.Value}");
        }

        public void HandlePickup(IPickable pickable) => 
            _playerPickup.HandlePickup(pickable);

        private Vector3 GetLook()
        {
            Vector2 direction = _simpleInput.LookAxis;
            Vector3 look = Vector3.zero;

            if(direction.magnitude >= 0.1f)
                look = new Vector3(direction.x, 0, direction.y).normalized;

            return look;
        }

        [ServerRpc]
        public void HandleCollectingOnServerRpc()
        {
            Accrue(1);
        }

        [ServerRpc]
        private void UpdateMovementAxisOnServerRpc(Vector3 movementAxis, ServerRpcParams serverRpcParams = default)
        {
            _movementAxis = movementAxis;
        }
        
        [ServerRpc]
        private void UpdateLookOnServerRpc(Vector3 lookDirection, ServerRpcParams serverRpcParams = default)
        {
            if (lookDirection.magnitude >= 0.1f)
                _lookDirection = lookDirection;
        }
    }
}