using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cube.Picked.Spawner
{
    public class CubeSpawner
    {
        private const float MinSpawnTime = 1f;
        private const float MaxSpawnTime = 5f;
        private const int StartCount = 20;
        private const int MaxSpawnAttempts = 50;

        private readonly CubeSpawnArea _spawnArea;
        private readonly CubeFactory _cubeFactory;
        private readonly MonoBehaviour _mono;

        private readonly IBoundable[] _damageBoundables;
        private List<IBoundable> _cubeBoundables = new List<IBoundable>();

        private int _spawnAttemptsCounter;

        public CubeSpawner(CubeSpawnArea spawnArea, IBoundable[] damageBoundables, MonoBehaviour mono)
        {
            _spawnArea = spawnArea;
            _damageBoundables = damageBoundables;
            _mono = mono;
            
            _cubeFactory = new CubeFactory(StartCount);

            _cubeFactory.OnCleaned += OnCubeCleaned;
        }

        public void Initialize()
        {
            for (int i = 0; i < StartCount; i++) 
                SafeSpawn();
        }

        public void Clean()
        {
            _cubeFactory.CleanAll();
            
            _mono.StopAllCoroutines();
        }

        private void SafeSpawn()
        {
            _spawnAttemptsCounter = 0;
            
            Spawn();
        }
        
        private void Spawn()
        {
            _spawnAttemptsCounter++;
            
            Vector3 position = _spawnArea.RandomPosition;
            
            if (CheckPosition(in position))
            {
                PickedCube pickedCube = _cubeFactory.Take(position);

                pickedCube.OnPicked += OnCubePicked;
                
                _cubeBoundables.Add(pickedCube);
                return;
            }

            if (_spawnAttemptsCounter > MaxSpawnAttempts)
            {
                Debug.LogWarning("Not found place");
                
                return;
            }
                
           Spawn();
        }

        private bool CheckPosition(in Vector3 position)
        {
            Bounds cubeBound = new Bounds(position, _cubeFactory.CubeSize);

            for (int i = 0; i < _damageBoundables.Length; i++)
            {
                if (cubeBound.Intersects(_damageBoundables[i].Bound))
                    return false;
            }

            for (int i = 0; i < _cubeBoundables.Count; i++)
            {
                if (cubeBound.Intersects(_cubeBoundables[i].Bound))
                    return false;
            }

            return true;
        }

        private void OnCubePicked(PickedCube pickedCube)
        {
            _cubeFactory.Put(pickedCube);

            _mono.StartCoroutine(SpawnByTimer());
        }

        private void OnCubeCleaned(PickedCube pickedCube)
        {
            pickedCube.OnPicked -= OnCubePicked;
            
            IBoundable boundable = pickedCube;

            if (_cubeBoundables.Contains(boundable))
                _cubeBoundables.Remove(boundable);
        }

        private IEnumerator SpawnByTimer()
        {
            yield return new WaitForSeconds(CalculateSpawnTime());
            
            SafeSpawn();
        }

        private static float CalculateSpawnTime() => 
            Random.Range(MinSpawnTime, MaxSpawnTime);
    }
}