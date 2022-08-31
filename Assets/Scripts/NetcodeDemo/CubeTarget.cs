using Unity.Netcode;
using UnityEngine;

namespace Characters.Player.NetcodeDemo
{
    public class CubeTarget : NetworkBehaviour
    {
        [SerializeField]
        private MeshRenderer _meshRenderer;

        [SerializeField] 
        private Material[] _materials;

        [ServerRpc]
        public void RandomTeleportServerRpc()
        {
            Vector3 position = new Vector3(Random.Range(4f, 8f), 1, Random.Range(-4f, 4f));
            transform.position = position;
        }

        [ClientRpc]
        public void ChangeColorClientRpc(int materialIndex)
        {
            _meshRenderer.material = _materials[materialIndex];
        }
    }
}