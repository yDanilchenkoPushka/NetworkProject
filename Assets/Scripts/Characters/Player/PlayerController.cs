using System;
using Characters.Enemy.Following;
using Cube.Picked;
using Damage;
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
        public Vector3 LookDirection => _lookDirection.Value;

        public NetworkVariable<int> CurrentScore => _currentScore;

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
        
        private PlayerHud _playerHud;

        private ISimpleInput _simpleInput;

        private PlayerAgent _playerAgent;
        private PlayerInteraction _playerInteraction;
        
        private PlayerInteraction _value;
        
        public static event Action<PlayerController> OnPlayerSpawned;
        public static event Action<PlayerController> OnPlayerDeSpawned;

        private NetworkVariable<Vector3> _movementAxis = new NetworkVariable<Vector3>(Vector3.zero,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        private NetworkVariable<Vector3> _lookDirection = new NetworkVariable<Vector3>(Vector3.zero,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private NetworkVariable<int> _currentScore = new NetworkVariable<int>();
        private NetworkVariable<NetworkString> _playerName = new NetworkVariable<NetworkString>();

        private void OnValidate()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            Debug.Log($"Spawn player {NetworkManager.LocalClientId}; IsHost: {NetworkManager.IsHost}");
            
            _simpleInput = AllServices.Container.Single<ISimpleInput>();

            _playerAgent = new PlayerAgent();
            _playerAgent.Construct(transform);
            
            _playerInteraction = new PlayerInteraction(_interactionTrigger, this);
        
            _playerMovement.Construct(_rigidbody, _playerAgent);
            _playerPickup.Construct(transform, _rigidbody, _collider, this);
            
            //_meshRenderer.material = _materials[materialIndex];

            if (IsOwner)
            {
                _lookDirection.Value = transform.forward;
                
                _simpleInput.OnInteracted += ClickInteract;
            }

            if (IsHost) 
                _playerName.Value = $"Player {OwnerClientId}";

            CreatePlayerHud();

            OnPlayerSpawned?.Invoke(this);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            OnPlayerDeSpawned?.Invoke(this);
            
            if(IsOwner)
                _simpleInput.OnInteracted -= ClickInteract;
            
            Destroy(_playerHud.gameObject);
        }

        private void OnDestroy() => 
            DeInitialize();

        private void DeInitialize()
        {
            //_playerInteraction.DeInitialize();
            
            _simpleInput = null;
        }


        // public void Tick()
        // {
        //     _playerAgent.Tick();
        //     UpdateLook();
        //     _playerPickup.Tick();
        // }

        private void Update()
        {
            _playerHud.UpdatePosition(transform.position);

            if (IsHost)
            {
                _playerPickup.Tick();
            }

            if (IsOwner)
            {
                Vector3 look = GetLook();
                if (look.magnitude >= 0.1f)
                    _lookDirection.Value = look;
            
                _movementAxis.Value = _simpleInput.MovementAxis;
            }
        }

        private void FixedUpdate()
        {
            if (!IsHost)
                return;
            
            _playerMovement.Move(_movementAxis.Value);
        }


        // public void Spawn(Vector3 at)
        // {
        //     transform.position = at;
        //     transform.rotation = Quaternion.identity;
        // }

        [ClientRpc]
        public void SpawnClientRpc(ulong localClientId, Vector3 at)
        {
            transform.position = at;
            transform.rotation = Quaternion.identity;
        }

        public void TakeDamage() => 
            OnDamaged?.Invoke();

        public void Accrue(int score)
        {
            _currentScore.Value += score;
        
            OnScoreUpdated?.Invoke(_currentScore.Value);
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

        [ServerRpc(RequireOwnership = false)]
        public void HandleCollectingOnServerRpc()
        {
            Accrue(1);
        }

        private void ClickInteract()
        {
            if (!IsOwner)
                return;

            ClickInteractOnServerRpc();
        }

        [ServerRpc]
        private void ClickInteractOnServerRpc()
        {
            Debug.Log($"Player {OwnerClientId} interact!");
            
            _playerInteraction.Interact();
        }

        private void CreatePlayerHud()
        {
            _playerHud = Instantiate(Resources.Load<PlayerHud>("PlayerHud"));
            _playerHud.UpdateLabel(_playerName.Value);
            _playerHud.UpdatePosition(transform.position);
        }
    }
}