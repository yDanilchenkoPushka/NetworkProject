using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Characters.Player.NetcodeDemo
{
    public class NetworkUI : MonoBehaviour
    {
        public event Action OnHostStarted;
        public event Action OnClientStarted;
        public event Action OnServerStarted;

        [SerializeField]
        private TextMeshProUGUI _labelMode;
        
        [SerializeField]
        private Button _startHostButton;
        
        [SerializeField]
        private Button _startClientButton;
        
        [SerializeField]
        private Button _startServerButton;

        public void Initialize()
        {
            _startHostButton.onClick.AddListener(() => OnHostStarted?.Invoke());
            _startClientButton.onClick.AddListener(() => OnClientStarted?.Invoke());
            _startServerButton.onClick.AddListener(() => OnServerStarted?.Invoke());
        }

        public void UpdateLabelMode(string mode) => 
            _labelMode.text = $"Mode: {mode}";

        public void HideButtons()
        {
            _startHostButton.gameObject.SetActive(false);
            _startClientButton.gameObject.SetActive(false);
            _startServerButton.gameObject.SetActive(false);
        }
    }
}