using System;
using System.Collections;
using Characters.Player;
using DG.Tweening;
using Interactive;
using Services;
using Unity.Netcode;
using UnityEngine;

namespace InteractiveObjects
{
    public class Door : MonoBehaviour, IInteractable
    {
        public event Action OnOpened;
        public bool IsDirty => _isDirty;
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
        private bool _isDirty;

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
            _isDirty = true;
            
            if(CanInteract) 
                SetOutline(true);
        }

        public void ExitInteractive()
        {
            _isDirty = false;

            SetOutline(false);
        }

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