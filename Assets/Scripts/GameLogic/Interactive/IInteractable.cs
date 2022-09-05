using UnityEngine;

namespace Interactive
{
    public interface IInteractable
    {
        bool IsDirty { get; }
        bool CanInteract { get; }
        Vector3 Position { get; }
        void EnterInteractive();
        void ExitInteractive();
        void Interact(object sender);
    }
}