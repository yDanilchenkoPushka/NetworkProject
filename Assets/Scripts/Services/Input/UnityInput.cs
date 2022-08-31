using System;
using Extensions;
using Other;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using DeviceType = Data.DeviceType;
using String = System.String;

namespace Services.Input
{
    public class UnityInput : ISimpleInput, IDeInitializable
    {
        public event Action<DeviceType> OnDeviceUpdated;
        public event Action OnTaped;
        public event Action OnInteracted;
        public event Action OnUpClicked;
        public event Action OnDownClicked;

        public Vector2 MovementAxis => new Vector2(
            _controls.Gameplay.Movement_hor.ReadValue<float>(),
            _controls.Gameplay.Movement_ver.ReadValue<float>());

        public Vector2 LookAxis => _controls.Gameplay.Look.ReadValue<Vector2>();

        public DeviceType LastDevice
        {
            get => _lastDevice;
            
            private set
            {
                _lastDevice = value;
                
                OnDeviceUpdated?.Invoke(value);
            }
        }

        private readonly Controls _controls;
        private DeviceType _lastDevice;

        public UnityInput(TextMeshProUGUI logLabel)
        {
            _controls = new Controls();

            if (logLabel != null)
            {
                var devices = InputSystem.devices;
                string log = String.Empty;

                foreach (var device in devices)
                {
                    log += device.name + "\n";
                }
            
                logLabel.text = log;
            }

            _controls.Enable();
            
            _controls.MainMenu.Click.performed += OnClickButtonDown;
            _controls.MainMenu.Move_down.performed += OnDownButtonDown;
            _controls.MainMenu.Move_up.performed += OnUpButtonDown;
            _controls.MainMenu.Mouse_hor.performed += OnMouseMoved;
            _controls.MainMenu.Mouse_ver.performed += OnMouseMoved;
            
            _controls.LevelMenu.Start.performed += OnStartButtonDown;

            _controls.Gameplay.Interact.performed += Interact;

            InputSystem.onEvent += HandleInput;
        }

        private void HandleInput(InputEventPtr ptr, InputDevice device)
        {
            if (!ptr.IsA<StateEvent>() && !ptr.IsA<DeltaStateEvent>())
                return;
            
            var controls = device.allControls;
            var buttonPressPoint = InputSystem.settings.defaultButtonPressPoint;

            for (var i = 0; i < controls.Count; ++i)
            {
                var control = controls[i] as ButtonControl;
                
                if (control == null || control.noisy)
                    continue;
                
                if (control.ReadValueFromEvent(ptr, out var value) && value >= buttonPressPoint)
                {
                    LastDevice = GetDevice(control.device.name);

                    break;
                }
            }
        }

        public void DeInitialize()
        {
            _controls.Disable();
            
            _controls.MainMenu.Click.performed -= OnClickButtonDown;
            _controls.MainMenu.Move_down.performed -= OnDownButtonDown;
            _controls.MainMenu.Move_up.performed -= OnUpButtonDown;
            _controls.MainMenu.Mouse_hor.performed -= OnMouseMoved;
            _controls.MainMenu.Mouse_ver.performed -= OnMouseMoved;
            
            _controls.LevelMenu.Start.performed -= OnStartButtonDown;
        }

        private void OnMouseMoved(InputAction.CallbackContext context)
        {
            float value = context.ReadValue<float>();
            value = Mathf.Abs(value);
            
            float mouseOffsetPoint = 500f;

            if (value >= mouseOffsetPoint) 
                LastDevice = DeviceType.Mouse;
        }

        private void OnClickButtonDown(InputAction.CallbackContext context) => 
            OnTaped?.Invoke();

        private void OnStartButtonDown(InputAction.CallbackContext context) => 
            OnTaped?.Invoke();

        private void OnUpButtonDown(InputAction.CallbackContext context) => 
            OnUpClicked?.Invoke();

        private void OnDownButtonDown(InputAction.CallbackContext data) => 
            OnDownClicked?.Invoke();

        private void Interact(InputAction.CallbackContext context) => 
            OnInteracted?.Invoke();

        private DeviceType GetDevice(string name)
        {
            //Debug.Log(layout);
            
            if (name.IsMatch("keyboard"))
                return DeviceType.Keyboard;
            
            if (name.IsMatch("gamepad"))
                return DeviceType.Gamepad;
            
            if(name.IsMatch("mouse"))
                return DeviceType.Mouse;

            return DeviceType.UnknownDevice;
        }
    }
}