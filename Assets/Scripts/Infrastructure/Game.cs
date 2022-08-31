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
using UnityEngine;

namespace Infrastructure
{
    public class Game : MonoBehaviour
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

        [SerializeField]
        private PathData _path;
    
        [SerializeField]
        private FollowingNPCController _followingNpc;
        
        [SerializeField]
        private PatrolNPCController _patrolNpc;
        
        [SerializeField]
        private BloodhoundNPCController _bloodhoundNpc;

        private PlayerController _playerController;
        private CubeSpawner _cubeSpawner;
        private ISceneLoader _sceneLoader;
        private ISimpleInput _simpleInput;

        private bool HasPlayer => _playerController != null;

        private void Awake()
        {
            _simpleInput = AllServices.Container.Single<ISimpleInput>();
            _sceneLoader = AllServices.Container.Single<ISceneLoader>();

            _cubeSpawnArea.Initialize();
            
            _cubeSpawner = new CubeSpawner(_cubeSpawnArea, _damageZones, this);
            _cubeSpawner.Initialize();
        
            _deviceBar.Construct(_simpleInput);
            _tipsBar.Construct(_simpleInput, _camera);
            _arrowBar.Initialize();

            _followingNpc.Construct(AllServices.Container.Single<ITickProcessor>());
            _patrolNpc.Construct(_path);
            _bloodhoundNpc.Construct(_cubeSpawnArea);
        
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

            _followingNpc.DeInitialize();
            
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