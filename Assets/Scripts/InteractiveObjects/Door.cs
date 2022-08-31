using System;
using System.Collections;
using DG.Tweening;
using Interactive;
using UnityEngine;

namespace InteractiveObjects
{
    public class Door : MonoBehaviour, IInteractable
    {
        public event Action OnOpened;
        
        public bool CanInteract => !_isOpened;
        public Vector3 Position => transform.position;
        
        [SerializeField]
        private float _animationTime;

        [SerializeField] 
        private float _closingTime;

        [SerializeField]
        private Outline _outline;

        private Tween _animation;
        private bool _isOpened;

        public void Awake()
        {
            _animation = transform
                .DOMoveY(-5f, _animationTime)
                .SetAutoKill(false)
                .Pause();
            
            SetOutline(false);
        }

        public void EnterInteractive()
        {
            if(CanInteract) 
                SetOutline(true);
        }

        public void ExitInteractive() => 
            SetOutline(false);

        public void Interact(object sender) => 
            Open();

        private void SetOutline(bool isOutline) => 
            _outline.enabled = isOutline;

        [ContextMenu("Open")]
        private void Open()
        {
            OnOpened?.Invoke();
            
            _animation.PlayForward();
            _isOpened = true;
            
            SetOutline(false);

            StartCoroutine(Close());
        }

        private IEnumerator Close()
        {
            yield return new WaitForSeconds(_closingTime);
            
            _animation.PlayBackwards();

            _isOpened = false;
            
            SetOutline(false);
        }
    }
}