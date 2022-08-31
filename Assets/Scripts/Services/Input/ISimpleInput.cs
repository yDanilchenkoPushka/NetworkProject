using System;
using UnityEngine;
using DeviceType = Data.DeviceType;

namespace Services.Input
{
    public interface ISimpleInput : IService
    {
        event Action<DeviceType> OnDeviceUpdated;
        event Action OnTaped;
        event Action OnInteracted;

        event Action OnUpClicked;
        event Action OnDownClicked;

        Vector2 MovementAxis { get; }
        Vector2 LookAxis { get; }
        DeviceType LastDevice { get; }
    }
}