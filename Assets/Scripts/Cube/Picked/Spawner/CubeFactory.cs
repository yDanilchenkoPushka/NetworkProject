using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cube.Picked.Spawner
{
    public class CubeFactory
    {
        private readonly CubeEffectFactory _cubeEffectFactory;
        public event Action<PickedCube> OnCleaned;

        public Vector3 CubeSize => _pickedCubePrefab.Size;

        private readonly PickedCube _pickedCubePrefab;

        private readonly List<Data> _pool = new List<Data>();

        private readonly Transform _root;

        public CubeFactory(int count, CubeEffectFactory cubeEffectFactory)
        {
            _cubeEffectFactory = cubeEffectFactory;
            _pickedCubePrefab = Resources.Load<PickedCube>("PickedCube");

            //_root = new GameObject("[Cube_Pool]").transform;

            Expand(count);
        }

        public PickedCube Take(in Vector3 at)
        {
            if (TryGetFree(out Data data))
            {
                data.SetDirty();

                PickedCube pickedCube = data.PickedCube;
                pickedCube.SpawnOnClientRpc(at);

                return pickedCube;
            }

            Expand(1);

            return Take(in at);
        }

        public void Put(PickedCube pickedCube)
        {
            if (TryFind(pickedCube, out Data data))
            {
                DeSpawn(data);

                return;
            }

            throw new Exception("Unknown cube");
        }

        private bool TryFind(PickedCube target, out Data data)
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                if (_pool[i].PickedCube == target)
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
                PickedCube pickedCube = Object.Instantiate(_pickedCubePrefab);

                pickedCube.GetComponent<NetworkObject>().Spawn();

                pickedCube.InitializeOnClientRpc();

                _pool.Add(new Data(pickedCube));
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
            public readonly PickedCube PickedCube;
            public bool IsDirty => _isDirty;

            private bool _isDirty;

            public Data(PickedCube pickedCube)
            {
                PickedCube = pickedCube;
                _isDirty = false;
            }

            public void SetDirty() =>
                _isDirty = true;

            public void Clean() =>
                _isDirty = false;
        }

        public void CleanAll()
        {
            // for (int i = 0; i < _pool.Count; i++)
            // {
            //     if (_pool[i].IsDirty) 
            //         DeSpawn(_pool[i]);
            // }
        }

        private void DeSpawn(Data data)
        {
            _cubeEffectFactory.Take(data.PickedCube.Position);
            
            data.PickedCube.DeSpawnOnClientRpc();
            data.Clean();

            OnCleaned?.Invoke(data.PickedCube);
        }
    }
}