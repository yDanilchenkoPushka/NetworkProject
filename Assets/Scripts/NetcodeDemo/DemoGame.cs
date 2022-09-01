using System.Collections.Generic;
using Characters.Player;
using Cube.Picked.Spawner;
using Damage;
using Data;
using InteractiveObjects;
using Services;
using Services.Input;
using Services.Scene;
using UI.Bars;
using UI.Bars.Tips;
using Unity.Netcode;
using UnityEngine;

namespace Infrastructure
{
    public class DemoGame : NetworkBehaviour
    {
        [SerializeField]
        private SpawnPoint _spawnPoint;

        [SerializeField]
        private CubeSpawnArea _cubeSpawnArea;

        [SerializeField]
        private DamageZone[] _damageZones;

        [SerializeField]
        private ScoreBar _scoreBar;

        [SerializeField]
        private DeviceBar _deviceBar;

        [SerializeField]
        private TipsBar _tipsBar;
    
        [SerializeField]
        private ArrowBar _arrowBar;

        [SerializeField]
        private Door _door;

        [SerializeField]
        private Camera _camera;
        
        private CubeSpawner _cubeSpawner;
        private ISceneLoader _sceneLoader;
        private ISimpleInput _simpleInput;

        private NetworkManager _networkManager;

        private List<PlayerController> _players = new List<PlayerController>();

        private void Awake()
        {
            _networkManager = NetworkManager.Singleton;
            
            _simpleInput = AllServices.Container.Single<ISimpleInput>();
            _sceneLoader = AllServices.Container.Single<ISceneLoader>();

            _cubeSpawnArea.Initialize();
            
            _cubeSpawner = new CubeSpawner(_cubeSpawnArea, _damageZones, this);
            _cubeSpawner.Initialize();
        
            _deviceBar.Construct(_simpleInput);
            _tipsBar.Construct(_simpleInput, _camera);
            _arrowBar.Initialize();

            _simpleInput.OnTaped += ClickCreatePlayer;

            _door.OnOpened += OnDoorOpened;

            PlayerController.OnPlayerSpawned += OnPlayerSpawned;
            PlayerController.OnPlayerDeSpawned += OnPlayerDeSpawned;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            Debug.Log($"Spawn DemoGame: {_networkManager.LocalClientId}; IsHost: {_networkManager.IsHost}");
        }

        private void OnDestroy()
        {
            _deviceBar.DeInitialize();
            _tipsBar.DeInitialize();

            _simpleInput.OnTaped -= ClickCreatePlayer;
        
            _door.OnOpened -= OnDoorOpened;
            
            PlayerController.OnPlayerSpawned -= OnPlayerSpawned;
            PlayerController.OnPlayerDeSpawned -= OnPlayerDeSpawned;
        }

        private void Update()
        {
            _tipsBar.Tick();
            _arrowBar.Tick();

            for (int i = 0; i < _players.Count; i++) 
                _players[i].Tick();
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < _players.Count; i++) 
                _players[i].FixedTick();
        }

        private void ClickCreatePlayer()
        {
            Debug.Log("ClickCreatePlayer");
            CreatePlayerServerRpc(_networkManager.LocalClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void CreatePlayerServerRpc(ulong localClientId)
        {
            string m = _networkManager.IsHost ? "Host" : "NotHost";
            Debug.Log($"Create player in {m}");

            if (_networkManager.IsHost || _networkManager.IsServer)
            {
                PlayerController playerControllerPrefab = Resources.Load<PlayerController>("Player");
            
                PlayerController playerController = Instantiate(playerControllerPrefab);
                
                _players.Add(playerController);
                
                NetworkObject networkObject = playerController.gameObject.GetComponent<NetworkObject>();
                networkObject.Spawn();
                
                playerController.SpawnClientRpc(localClientId, _spawnPoint.Position);

                //_scoreBar.Construct(_playerController);
                //_tipsBar.Initialize(_playerController);

        
                //playerController.OnDamaged += KillPlayerController;
            }
        }

        private void KillPlayerController()
        {
            //_scoreBar.DeInitialize();
            
            //_playerController.OnDamaged -= KillPlayerController;

            LoadMainMenu();
        }

        private void LoadMainMenu() => 
            _sceneLoader.Load(SceneInfos.MAIN_MENU);
        
        private void OnDoorOpened()
        {
            _sceneLoader.LoadAdditive(SceneInfos.LEVEL_2);
            
            _door.OnOpened -= OnDoorOpened;
        }
        
        [ContextMenu("Print")]
        public void Print()
        {
            Debug.Log(IsOwner);
        }

        private void OnPlayerSpawned(PlayerController controller)
        {
            Debug.Log($"OnPlayerSpawned: {controller.NetworkManager.LocalClientId}");
            
            _arrowBar.Construct(controller, controller);
        }
        
        private void OnPlayerDeSpawned(PlayerController controller)
        {
            Debug.Log($"OnPlayerDeSpawned: {controller.NetworkManager.LocalClientId}");
        }
    }
}