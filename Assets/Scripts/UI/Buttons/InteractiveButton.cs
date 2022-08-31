using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Buttons
{
    public abstract class InteractiveButton : MonoBehaviour, IPointerEnterHandler
    {
        public event Action<int> OnOvered;
        public event Action OnClicked;

        [SerializeField]
        private Button _button;
        
        [SerializeField]
        private Image _image;

        private Color _defaulColor;
        private int _id;

        public void Initialize(int id)
        {
            _id = id;
            _defaulColor = _image.color;
            
            _button.onClick.AddListener(Click);
        }

        public void DeInitialize() => 
            _button.onClick.RemoveListener(Click);

        public void Select(Color color) => 
            _image.color = color;

        public void UnSelect() => 
            _image.color = _defaulColor;

        public void Click() => 
            OnClicked?.Invoke();
        
        public void OnPointerEnter(PointerEventData eventData) => 
            OnOvered?.Invoke(_id);
    }
}