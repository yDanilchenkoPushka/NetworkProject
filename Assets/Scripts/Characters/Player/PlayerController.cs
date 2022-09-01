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
        private int _currentScore;

        private PlayerAgent _playerAgent;
        private PlayerInteraction _playerInteraction;
        
        private Vector3 _lookDirection;
        private PlayerInteraction _value;

        private bool _isConstructed;
        
        
        //private NetworkVariable<Vector3> _movementAxis = new NetworkVariable<Vector3>();
        // private NetworkVariable<Vector3> _movementAxisTest = new NetworkVariable<Vector3>(Vector3.zero,
        //     NetworkVariableReadPermission.Everyone,
        //     NetworkVariableWritePermission.);
        
        private Vector3 _movementAxis;
        private ulong _localClientId;
        public static event Action<PlayerController> OnPlayerSpawned;
        public static event Action<PlayerController> OnPlayerDeSpawned;

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
            if (_localClientId != NetworkManager.Singleton.LocalClientId)
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
            _currentScore += score;
        
            OnScoreUpdated?.Invoke(_currentScore);
        }

        public void HandlePickup(IPickable pickable) => 
            _playerPickup.HandlePickup(pickable);

        private Vector2 GetLook()
        {
            Vector2 look = _simpleInput.LookAxis;
            
            if(look.magnitude >= 0.1f)
                _lookDirection = new Vector3(look.x, 0, look.y).normalized;

            return _lookDirection;
        }

        public void HandleCollecting()
        {
            Accrue(1);
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdateMovementAxisOnServerRpc(Vector3 movementAxis, ServerRpcParams serverRpcParams = default)
        {
            _movementAxis = movementAxis;
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void UpdateLookOnServerRpc(Vector3 lookDirection, ServerRpcParams serverRpcParams = default)
        {
            _lookDirection = lookDirection;
        }
    }
}