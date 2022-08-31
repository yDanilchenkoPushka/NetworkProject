using System;
using System.Threading;
using Data;
using Infrastructure.Processors.Tick;
using Services;
using Services.Input;
using Services.Scene;
using TMPro;
using UI.Bars;
using UI.Buttons;
using Unity.Netcode;
//using Unity.GameCore;
using UnityEngine;

namespace Infrastructure
{
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _logLabel;
        
        [SerializeField]
        private TextMeshProUGUI _log;
        
        [SerializeField]
        private ButtonBar _buttonBar;

        [SerializeField]
        private DeviceBar _deviceBar;

        [SerializeField]
        private UserBar _userBar;

        private ExitHandler _exitHandler;

        private AllServices _services = new AllServices();

        //private XboxUser _xbUser;
        
        private Thread m_DispatchJob;
        private bool m_StopExecution;

        private void Awake()
        {
            RegisterServices();
            
            _exitHandler = new ExitHandler();
            
            _buttonBar.Construct(_services.Single<ISimpleInput>());
            _deviceBar.Construct(_services.Single<ISimpleInput>());
            
            //_userBar.Initialize();
            
            Initialize();
        }

        private void Update()
        {
            // if (_xbUser != null)
            //     _xbUser.Tick();
        }

        private void Initialize()
        {
            InteractiveButton interactiveButton;
            
            if (_buttonBar.TryGetButton<StartGameButton>(out interactiveButton))
                interactiveButton.OnClicked += LoadLevel;
            
            if (_buttonBar.TryGetButton<ExitButton>(out interactiveButton))
                interactiveButton.OnClicked += Exit;

            //XblInitialize();
        }

        private void DeInitialize()
        {
            _buttonBar.DeInitialize();
            _deviceBar.DeInitialize();
            //_userBar.DeInitialize();
            
            InteractiveButton interactiveButton;
            
            if (_buttonBar.TryGetButton<StartGameButton>(out interactiveButton))
                interactiveButton.OnClicked -= LoadLevel;
            
            if (_buttonBar.TryGetButton<ExitButton>(out interactiveButton))
                interactiveButton.OnClicked -= Exit;
            
            m_StopExecution = true;
        }

        private void OnDestroy() => 
            DeInitialize();

        private void LoadLevel()
        {
            ISceneLoader sceneLoader = _services.Single<ISceneLoader>();
            sceneLoader.Load(SceneInfos.LEVEL);
        }

        private void Exit() => 
            _exitHandler.Exit();

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
        _log.text = "RewiredInput";
        return new RewiredInput();
#else
            _log.text = "UnityInput";
            return new UnityInput(_logLabel);
#endif
        }

        // private void XblInitialize()
        // {
        //     Int32 hr = SDK.XGameRuntimeInitialize();
        //     if (hr == 0)
        //     {
        //         // start the async task dispatch thread
        //         m_StopExecution = false;
        //         m_DispatchJob = new Thread(DispatchGXDKTaskQueue) { Name = "GXDK Task Queue Dispatch" };
        //         m_DispatchJob.Start();
        //         
        //         hr = SDK.XBL.XblInitialize(GameCore.GameCoreSettings.SCID);
        //         if (hr == 0)
        //         {
        //             CreateUser();
        //         }
        //     }
        // }

        // private void CreateUser()
        // {
        //     _xbUser = new XboxUser();
        //     
        //     _userBar.Construct(_xbUser);
        //     
        //     _xbUser.AddDefaultUser();
        // }
        
        // void DispatchGXDKTaskQueue()
        // {
        //     // this will execute all GXDK async work
        //     while (m_StopExecution == false)
        //     {
        //         SDK.XTaskQueueDispatch(0);
        //         Thread.Sleep(32);
        //     }
        // }
    }
}