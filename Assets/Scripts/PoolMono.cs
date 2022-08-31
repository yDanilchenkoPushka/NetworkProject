using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class PoolMono<TMono> where TMono : MonoBehaviour
{
    private readonly TMono _prefab;
    private readonly Transform _container;
    private readonly bool _isDefaultActive;
    private readonly List<PoolElement> _poolElements = new List<PoolElement>();

    private Action<TMono> _objectCreated;
    private Action<TMono> _objectCleaned;
        
    private int _counter;

    public PoolMono(
        TMono prefab,
        Transform container,
        bool isDefaultActive = false)
    {
        _prefab = prefab;
        _container = container;
        _isDefaultActive = isDefaultActive;
    }
        
    public PoolMono(
        TMono prefab,
        string container,
        bool isDefaultActive = false)
    {
        _prefab = prefab;
        _container = new GameObject(container).transform;
        _isDefaultActive = isDefaultActive;
    }

    public PoolMono<TMono> AddCreateCallback(Action<TMono> objectCreated)
    {
        _objectCreated = objectCreated;
        return this;
    }

    public PoolMono<TMono> AddCleanupCallback(Action<TMono> objectCleaned)
    {
        _objectCleaned = objectCleaned;
        return this;
    }

    public PoolMono<TMono> Expand(int count)
    {
        for (int i = 0; i < count; i++)
        {
            TMono createdObject = Object.Instantiate(_prefab, _container);
            createdObject.gameObject.name = $"{_prefab.name}_{++_counter}";
                
            SetInContainer(createdObject);
            _poolElements.Add(new PoolElement(createdObject));
                
            _objectCreated?.Invoke(createdObject);
        }

        return this;
    }

    public TMono Take()
    {
        if (TryFindFreeElement(out PoolElement freeElement))
        {
            freeElement.IsUsed = true;
            return freeElement.Object;
        }
        
        Expand(1);
        return Take();
    }

    public void Put(TMono target)
    {
        if(TryFindLikeElement(target, out PoolElement foundElement))
        {
            foundElement.IsUsed = false;
            SetInContainer(target);

            return;
        }

        throw new NullReferenceException($"Don't found object in pool: {target}");
    }

    public void Clean()
    {
        for (int i = 0; i < _poolElements.Count; i++)
        {
            if (_poolElements[i].IsUsed)
            {
                _poolElements[i].IsUsed = false;
                SetInContainer(_poolElements[i].Object);
                    
                _objectCleaned?.Invoke(_poolElements[i].Object);
            }
        }
    }

    private bool TryFindFreeElement(out PoolElement element)
    {
        for (int i = 0; i < _poolElements.Count; i++)
        {
            if (_poolElements[i].IsUsed)
                continue;

            element = _poolElements[i];
            return true;
        }

        element = null;
        return false;
    }

    private bool TryFindLikeElement(TMono target, out PoolElement element)
    {
        for (int i = 0; i < _poolElements.Count; i++)
        {
            if (_poolElements[i].Object == target)
            {
                element = _poolElements[i];
                return true;
            } 
        }

        element = null;
        return false;
    }

    private void SetInContainer(TMono target)
    {
        target.gameObject.SetActive(_isDefaultActive);
            
        target.transform.SetParent(_container);
        //target.transform.parent = _container;
    }

    public void PrintInfo()
    {
        string poolObjects = "Pool objects:";
        string takenObjects = "Taken objects:";

        for (int i = 0; i < _poolElements.Count; i++)
        {
            string name = "\n" + _poolElements[i].Object.name;
                
            if (_poolElements[i].IsUsed)
                takenObjects += name;
            else
                poolObjects += name;
        }
            
        Debug.Log(poolObjects);
        Debug.Log(takenObjects);
    }
        
    private class PoolElement
    {
        public readonly TMono Object;
        public bool IsUsed;

        public PoolElement(TMono @object)
        {
            Object = @object;
            IsUsed = false;
        }
    }
}