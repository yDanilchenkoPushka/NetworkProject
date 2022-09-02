using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Characters.Player
{
    public class CubeEffect : NetworkBehaviour
    {
        public event Action<CubeEffect> OnDeSpawned;

        [SerializeField]
        private ParticleSystem _particleSystem;

        private bool _hasDelay;

        [ClientRpc]
        public void InitializeOnClientRpc()
        {
            gameObject.SetActive(false);
        }

        [ClientRpc]
        public void SpawnClientRpc(Vector3 at)
        {
            if (_hasDelay)
                return;

            transform.position = at;
            gameObject.SetActive(true);
            _particleSystem.Play();

            StartCoroutine(DeSpawn());
        }

        private IEnumerator DeSpawn()
        {
            _hasDelay = true;
            
            yield return new WaitForSeconds(_particleSystem.main.duration);
            
            _particleSystem.Stop();
            gameObject.SetActive(false);

            _hasDelay = false;
            
            OnDeSpawned?.Invoke(this);
        }
    }
}