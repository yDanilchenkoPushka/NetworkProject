using System;
using Other;
using Rewired;
using UnityEngine;
using DeviceType = Data.DeviceType;

namespace Services.Input
{
    public class RewiredInput : ISimpleInput, IDeInitializable
    {
        public event Action<DeviceType> OnDeviceUpdated;
        public event Action OnTaped;
        public event Action OnInteracted;
        public event Action OnUpClicked;
        public event Action OnDownClicked;

        public Vector2 MovementAxis => new Vector2(
            _player.GetAxis("Movement_hor"),
            _player.GetAxis("Movement_ver"));

        public Vector2 LookAxis => new Vector2(
            _player.GetAxis("Look_hor"),
            _player.GetAxis("Look_ver"));

        public DeviceType LastDevice
        {
            get => _lastDevice;
            
            private set
            {
                _lastDevice = value;
                
                OnDeviceUpdated?.Invoke(value);
            }
        }

        private readonly Rewired.Player _player;
        private DeviceType _lastDevice;

        public RewiredInput()
        {
            _player = ReInput.players.GetPlayer(0);
            
            _player.AddInputEventDelegate(OnUpButtonDown, UpdateLoopType.Update, "Move_up");
            _player.AddInputEventDelegate(OnDownButtonDown, UpdateLoopType.Update, "Move_down");
            _player.AddInputEventDelegate(OnTapButtonDown, UpdateLoopType.Update, "Click");

            _player.AddInputEventDelegate(OnStartButtonDown, UpdateLoopType.Update, "Start");
            
            _player.AddInputEventDelegate(OnInteractButtonDown, UpdateLoopType.Update, "Interact");
            
            _player.AddInputEventDelegate(UpdateDeviceInfo, UpdateLoopType.Update);
        }

        public void DeInitialize()
        {
            _player.RemoveInputEventDelegate(OnUpButtonDown, UpdateLoopType.Update, "Move_up");
            _player.RemoveInputEventDelegate(OnDownButtonDown, UpdateLoopType.Update, "Move_down");
            _player.RemoveInputEventDelegate(OnTapButtonDown, UpdateLoopType.Update, "Click");
            
            _player.RemoveInputEventDelegate(OnStartButtonDown, UpdateLoopType.Update, "Start");
            
            _player.RemoveInputEventDelegate(OnInteractButtonDown, UpdateLoopType.Update, "Interact");

            _player.RemoveInputEventDelegate(UpdateDeviceInfo, UpdateLoopType.Update);
        }

        private void OnTapButtonDown(InputActionEventData data)
        {
            if (data.GetButtonDown()) 
                OnTaped?.Invoke();
        }

        private void OnStartButtonDown(InputActionEventData data)
        {
            if (data.GetButtonDown()) 
                OnTaped?.Invoke();
        }

        private void UpdateDeviceInfo(InputActionEventData eventData)
        {
            CheckMouse();
            CheckJoysticks();
            CheckKeyboards();
        }

        private void OnUpButtonDown(InputActionEventData data)
        {
            if (data.GetButtonDown()) 
                OnUpClicked?.Invoke();
        }

        private void OnDownButtonDown(InputActionEventData data)
        {
            if (data.GetButtonDown()) 
                OnDownClicked?.Invoke();
        }

        private void CheckMouse()
        {
            var mouse = ReInput.controllers.Mouse;

            if (IsActiveMouse(mouse)) 
                LastDevice = DeviceType.Mouse;
        }

        private void CheckJoysticks()
        {
            var joysticks = ReInput.controllers.Joysticks;

            foreach (var joystick in joysticks)
            {
                if (IsActiveJoystick(joystick))
                {
                    LastDevice = DeviceType.Gamepad;

                    break;
                }
            }
        }

        private void CheckKeyboards()
        {
            var keyboard = ReInput.controllers.Keyboard;

            for (int i = 0; i < keyboard.buttonCount; i++)
            {
                if (keyboard.GetButtonDown(i))
                {
                    LastDevice = DeviceType.Keyboard;
      
                    break;
                }
            }
        }

        private bool IsActiveMouse(Mouse mouse)
        {
            for (int i = 0; i < mouse.buttonCount; i++)
            {
                if (mouse.GetButtonDown(i))
                    return true;
            }

            float offsetPoint = 0.05f;
            for (int i = 0; i < mouse.axisCount; i++)
            {
                if (mouse.GetAxis(i) >= offsetPoint)
                    return true;
            }

            for (int i = 0; i < mouse.axis2DCount; i++)
            {
                if (mouse.GetAxis2D(i).magnitude >= offsetPoint)
                    return true;
            }

            return false;
        }

        private bool IsActiveJoystick(Joystick joystick)
        {
            for (int i = 0; i < joystick.buttonCount; i++)
            {
                if (joystick.GetButtonDown(i))
                    return true;
            }

            float offsetPoint = 0.05f;
            for (int i = 0; i < joystick.axisCount; i++)
            {
                if (joystick.GetAxis(i) >= offsetPoint)
                    return true;
            }
            
            for (int i = 0; i < joystick.axis2DCount; i++)
            {
                if (joystick.GetAxis2D(i).magnitude >= offsetPoint)
                    return true;
            }

            return false;
        }

        private void OnInteractButtonDown(InputActionEventData eventData)
        {
            if (eventData.GetButtonDown())
                OnInteracted?.Invoke();
        }
    }
}