using System.Collections;
using System.Collections.Generic;
using Characters.Player;
using Cube.Picked.Spawner;
using Damage;
using Data;
using InteractiveObjects;
using Services;
using Services.Input;
using Services.Scene;
using TMPro;
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
        private Door _door;

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private Transform _cubePoolRoot;

        [SerializeField]
        private TextMeshProUGUI _playerInfo;

        private CubeSpawner _cubeSpawner;
        private ISceneLoader _sceneLoader;
        private ISimpleInput _simpleInput;
        private NetworkManager _networkManager;
        private List<PlayerController> _players = new List<PlayerController>();
        private List<ArrowBar> _arrowBars = new List<ArrowBar>();

        private PlayerController _playerController;

        private void Awake()
        {
            _networkManager = NetworkManager.Singleton;

            string info = $"Player {_networkManager.LocalClientId}; IsHost: {NetworkManager.IsHost}";
            _playerInfo.text = info;

            _simpleInput = AllServices.Container.Single<ISimpleInput>();
            _sceneLoader = AllServices.Container.Single<ISceneLoader>();

            _cubeSpawnArea.Initialize();

            if (_networkManager.IsHost)
            {
                _cubeSpawner = new CubeSpawner(_cubeSpawnArea, _damageZones, this, _cubePoolRoot);
                _cubeSpawner.Initialize();
            }
            
            _deviceBar.Construct(_simpleInput);
            _tipsBar.Construct(_simpleInput, _camera);

            _simpleInput.OnTaped += ClickCreatePlayer;

            _door.OnOpened += OnDoorOpened;

            PlayerController.OnPlayerSpawned += OnPlayerSpawned;
        }

        private void OnDestroy()
        {
            _deviceBar.DeInitialize();
            _tipsBar.DeInitialize();

            _simpleInput.OnTaped -= ClickCreatePlayer;
        
            _door.OnOpened -= OnDoorOpened;
            
            PlayerController.OnPlayerSpawned -= OnPlayerSpawned;
        }

        private void Update()
        {
            if(!_networkManager.IsHost)
                return;
            
            _tipsBar.Tick();

            for (int i = 0; i < _players.Count; i++) 
                _players[i].Tick();

            for (int i = 0; i < _arrowBars.Count; i++) 
                _arrowBars[i].Tick();
        }

        private void FixedUpdate()
        {
            if(!_networkManager.IsHost)
                return;
            
            for (int i = 0; i < _players.Count; i++) 
                _players[i].FixedTick();
        }

        private void ClickCreatePlayer() => 
            CreatePlayerServerRpc(_networkManager.LocalClientId);

        [ServerRpc(RequireOwnership = false)]
        private void CreatePlayerServerRpc(ulong localClientId)
        {
            if (_networkManager.IsHost || _networkManager.IsServer)
            {
                PlayerController playerControllerPrefab = Resources.Load<PlayerController>("Player");
            
                PlayerController playerController = Instantiate(playerControllerPrefab);
                
                _players.Add(playerController);
                
                playerController.gameObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(localClientId, true);

                playerController.SpawnClientRpc(localClientId, _spawnPoint.Position);

                ArrowBar arrowBar = CreateArrowBar();
                arrowBar.Construct(playerController, playerController);
                
                arrowBar.GetComponent<NetworkObject>().SpawnWithOwnership(localClientId, true);
                
                //_tipsBar.Initialize(_playerController);

                //playerController.OnDamaged += KillPlayerController;
            }
        }

        private ArrowBar CreateArrowBar()
        {
            ArrowBar arrowBarPrefab = Resources.Load<ArrowBar>("ArrowBar");
            
            ArrowBar arrowBar = Instantiate(arrowBarPrefab);
                
            _arrowBars.Add(arrowBar);

            return arrowBar;
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

        private void OnPlayerSpawned(PlayerController playerController)
        {
            if (playerController.IsOwner)
            {
                _simpleInput.OnTaped -= ClickCreatePlayer;
                
                _playerController = playerController;
                PlayerController.OnPlayerSpawned -= OnPlayerSpawned;

                _scoreBar.Construct(_playerController);
            }
        }
        
       
    }
}