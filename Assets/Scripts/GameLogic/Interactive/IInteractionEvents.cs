using System;
using Interactive;

namespace Characters.Player
{
    public interface IInteractionEvents
    {
        event Action<IInteractable> OnEntered;
        event Action<IInteractable> OnExited;
        event Action OnUpdated;
    }
}