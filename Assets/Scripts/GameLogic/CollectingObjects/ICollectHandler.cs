using Unity.Netcode;

namespace Cube.Picked
{
    public interface ICollectHandler
    {
        [ServerRpc]
        void HandleCollectingOnServerRpc();
    }
}