using Services.Input;
using TMPro;
using UnityEngine;
using DeviceType = Data.DeviceType;

namespace UI.Bars
{
    public class DeviceBar : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _title;

        private ISimpleInput _simpleInput;

        public void Construct(ISimpleInput simpleInput)
        {
            _simpleInput = simpleInput;
            
            simpleInput.OnDeviceUpdated += UpdateLabel;
        }

        public void DeInitialize() => 
            _simpleInput.OnDeviceUpdated -= UpdateLabel;

        private void UpdateLabel(DeviceType deviceType) => 
            _title.text = deviceType.ToString();
    }
}