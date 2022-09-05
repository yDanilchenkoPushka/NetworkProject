using System;
using System.Collections.Generic;
using Interactive;
using UnityEngine;

namespace Characters.Player
{
    public class PlayerInteraction : IInteractionEvents
    {
        public event Action<IInteractable> OnEntered;
        public event Action<IInteractable> OnExited;
        public event Action OnUpdated;

        private readonly CheckerTrigger _interactionTrigger;

        private readonly List<IInteractable> _interactables = new List<IInteractable>();
        private readonly object _sender;

        public PlayerInteraction(CheckerTrigger interactionTrigger, object sender)
        {
            _interactionTrigger = interactionTrigger;
            _sender = sender;

            _interactionTrigger.OnEntered += Enter;
            _interactionTrigger.OnExited += Exit;
        }

        public void DeInitialize()
        {
            _interactionTrigger.OnEntered -= Enter;
            _interactionTrigger.OnExited -= Exit;
        }

        private void Enter(Collider other)
        {
            // if (!NetworkManager.Singleton.IsHost)
            //     return;
            
            if (other.attachedRigidbody == null)
                return;
            
            if (other.attachedRigidbody.TryGetComponent<IInteractable>(out IInteractable interaction))
            {
                if (_interactables.Contains(interaction))
                    return;

                if (interaction.IsDirty)
                    return;
                
                _interactables.Add(interaction);

                interaction.EnterInteractive();
                
                OnEntered?.Invoke(interaction);
                OnUpdated?.Invoke();
            }
        }

        private void Exit(Collider other)
        {
            // if (!NetworkManager.Singleton.IsHost)
            //     return;
            
            if (other.attachedRigidbody == null)
                return;
            
            if (other.attachedRigidbody.TryGetComponent<IInteractable>(out IInteractable interaction))
            {
                if (_interactables.Contains(interaction))
                {
                    _interactables.Remove(interaction);
                    
                    interaction.ExitInteractive();
                    
                    OnUpdated?.Invoke();
                    OnExited?.Invoke(interaction);
                }
            }
        }
        
        public void Interact()
        {
            for (int i = 0; i < _interactables.Count; i++)
            {
                if(_interactables[i].CanInteract)
                    _interactables[i].Interact(_sender);
            }
        }
    }
}