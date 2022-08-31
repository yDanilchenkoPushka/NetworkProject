using UnityEngine;

namespace Interactive
{
    public interface IInteractable
    {
        bool CanInteract { get; }
        Vector3 Position { get; }
        void EnterInteractive();
        void ExitInteractive();
        void Interact(object sender);
    }
}