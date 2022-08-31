using System;
using UnityEngine;

namespace UI.Bars
{
    [RequireComponent(typeof(Canvas))]
    public class ArrowBar : MonoBehaviour
    {
        private const float HeightOffset = 0.105f;

        [SerializeField, HideInInspector]
        private Canvas _canvas;
        
        [SerializeField, HideInInspector]
        private RectTransform _canvasRectTransform;
        
        private ILookable _lookable;
        private IPositionable _positionable;

        private bool IsValidate => _lookable != null;

        private void OnValidate()
        {
            _canvas = GetComponent<Canvas>();
            _canvasRectTransform = GetComponent<RectTransform>();
        }

        public void Initialize() => 
            Hide();

        public void Construct(ILookable lookable, IPositionable positionable)
        {
            _lookable = lookable;
            _positionable = positionable;
            
            Show();
        }

        public void Tick()
        {
            if (IsValidate)
            {
                Place();
                Turn();
            }
        }

        private void Show() => 
            _canvas.enabled = true;

        private void Hide() => 
            _canvas.enabled = false;

        private void Place()
        {
            Vector3 position = new Vector3(_positionable.Position.x, HeightOffset, _positionable.Position.z);

            _canvasRectTransform.position = position;
        }

        private void Turn()
        {
            Quaternion rotation = Quaternion.LookRotation(_lookable.LookDirection * -1f, Vector3.up);

            _canvasRectTransform.rotation = rotation;
        }
    }
}