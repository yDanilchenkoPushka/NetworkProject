using System;
using System.Collections.Generic;
using Characters.Player;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cube.Picked.Spawner
{
    public class CubeEffectFactory
    {
        private readonly CubeEffect _cubeEffectPrefab;

        private readonly List<Data> _pool = new List<Data>();

        private readonly Transform _root;

        public CubeEffectFactory()
        {
            _cubeEffectPrefab = Resources.Load<CubeEffect>("CubeEffect");

            Expand(1);
        }

        public CubeEffect Take(in Vector3 at)
        {
            if (TryGetFree(out Data data))
            {
                data.SetDirty();

                CubeEffect cubeEffect = data.CubeEffect;
                cubeEffect.SpawnClientRpc(at);

                cubeEffect.OnDeSpawned += Put;

                return cubeEffect;
            }

            Expand(1);

            return Take(in at);
        }

        private void Put(CubeEffect cubeEffect)
        {
            if (TryFind(cubeEffect, out Data data))
            {
                data.Clean();

                return;
            }

            throw new Exception("Unknown cube");
        }

        private bool TryFind(CubeEffect target, out Data data)
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                if (_pool[i].CubeEffect == target)
                {
                    data = _pool[i];
                    return true;
                }
            }

            data = default;
            return false;
        }

        private void Expand(int count)
        {
            for (int i = 0; i < count; i++)
            {
                CubeEffect cubeEffect = Object.Instantiate(_cubeEffectPrefab);

                cubeEffect.GetComponent<NetworkObject>().Spawn();

                cubeEffect.InitializeOnClientRpc();

                _pool.Add(new Data(cubeEffect));
            }
        }

        private bool TryGetFree(out Data data)
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                if (_pool[i].IsDirty)
                    continue;

                data = _pool[i];
                return true;
            }

            data = default;
            return false;
        }

        private class Data
        {
            public readonly CubeEffect CubeEffect;
            public bool IsDirty => _isDirty;

            private bool _isDirty;

            public Data(CubeEffect cubeEffect)
            {
                CubeEffect = cubeEffect;
                _isDirty = false;
            }

            public void SetDirty() =>
                _isDirty = true;

            public void Clean() =>
                _isDirty = false;
        }
    }
}