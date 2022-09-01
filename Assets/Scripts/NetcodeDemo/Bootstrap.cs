using Data;
using Infrastructure.Processors.Tick;
using Services;
using Services.Input;
using Services.Scene;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Characters.Player.NetcodeDemo
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField]
        private Button _createGameButton;

        [SerializeField]
        private Button _connectGameButton;
        
        private AllServices _services = new AllServices();
        private NetworkManager _networkManager;

        private void Start()
        {
            _networkManager = NetworkManager.Singleton;
            
            RegisterServices();
            
            _createGameButton.onClick.AddListener(CreateGame);
            _connectGameButton.onClick.AddListener(ConnectGame);
        }

        private void CreateGame()
        {
            _networkManager.StartHost();

            LoadLevel();
        }

        private void ConnectGame()
        {
            _networkManager.StartClient();

            //LoadLevel();
        }

        private void RegisterServices()
        {
            if(!_services.IsRegistered<ITickProcessor>())
                _services.RegisterSingle<ITickProcessor>(CreateTickProcessor());
            
            if(!_services.IsRegistered<ISimpleInput>())
                _services.RegisterSingle<ISimpleInput>(InputService());
        
            if(!_services.IsRegistered<ISceneLoader>())
                _services.RegisterSingle<ISceneLoader>(new SceneLoader());
        }
        
        private ITickProcessor CreateTickProcessor()
        {
            TickProcessor tickProcessor = new GameObject("TickProcessor").AddComponent<TickProcessor>();
            DontDestroyOnLoad(tickProcessor);

            return tickProcessor;
        }

        private ISimpleInput InputService()
        {
#if REWIRED_INPUT
        return new RewiredInput();
#else
            return new UnityInput(null);
#endif
        }
        
        private void LoadLevel()
        {
            _networkManager.SceneManager.LoadScene(SceneInfos.DEMO_LEVEL, LoadSceneMode.Single);
        }
    }
}