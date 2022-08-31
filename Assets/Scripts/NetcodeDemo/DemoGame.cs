using Characters.Enemy.Bloodhound;
using Characters.Enemy.Following;
using Characters.Enemy.Patrol;
using Characters.Player;
using Cube.Picked.Spawner;
using Damage;
using Data;
using Infrastructure.Processors.Tick;
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
    public class DemoGame : MonoBehaviour
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

        private PlayerController _playerController;
        private CubeSpawner _cubeSpawner;
        private ISceneLoader _sceneLoader;
        private ISimpleInput _simpleInput;

        private bool HasPlayer => _playerController != null;
        
        private NetworkManager _networkManager;

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

            _simpleInput.OnTaped += CreatePlayer;

            _door.OnOpened += OnDoorOpened;
        }

        private void OnDestroy()
        {
            _deviceBar.DeInitialize();
            _tipsBar.DeInitialize();

            _simpleInput.OnTaped -= CreatePlayer;
        
            _door.OnOpened -= OnDoorOpened;
        }

        private void Update()
        {
            _tipsBar.Tick();
            _arrowBar.Tick();
        }

        private void CreatePlayer()
        {
            if (HasPlayer)
                return;
            
            PlayerController playerControllerPrefab = Resources.Load<PlayerController>("Player");
            
            _playerController = Instantiate(playerControllerPrefab);
            _playerController.Construct(_simpleInput);
            _playerController.Spawn(_spawnPoint.Position);

            NetworkObject networkObject = _playerController.gameObject.GetComponent<NetworkObject>();
            networkObject.Spawn();

            _scoreBar.Construct(_playerController);
            _tipsBar.Initialize(_playerController);
            _arrowBar.Construct(_playerController, _playerController);
        
            _playerController.OnDamaged += KillPlayerController;
            
            _simpleInput.OnTaped -= CreatePlayer;
        }

        private void KillPlayerController()
        {
            _scoreBar.DeInitialize();
            
            _playerController.OnDamaged -= KillPlayerController;

            LoadMainMenu();
        }

        private void LoadMainMenu() => 
            _sceneLoader.Load(SceneInfos.MAIN_MENU);
        
        private void OnDoorOpened()
        {
            _sceneLoader.LoadAdditive(SceneInfos.LEVEL_2);
            
            _door.OnOpened -= OnDoorOpened;
        }
    }
}