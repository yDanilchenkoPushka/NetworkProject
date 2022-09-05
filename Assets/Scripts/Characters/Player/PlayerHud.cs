using System;
using TMPro;
using UnityEngine;

namespace Characters.Player
{
    public class PlayerHud : MonoBehaviour
    {
        [SerializeField]
        private Transform _hud;
        
        [SerializeField]
        private TextMeshProUGUI _label;

        public void UpdateLabel(string info) => 
            _label.text = info;

        public void UpdatePosition(Vector3 at)
        {
            _hud.position = at + Vector3.up * 1.5f;
            _hud.rotation = Quaternion.identity;
        }
    }
}