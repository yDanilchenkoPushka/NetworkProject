using System;
using Services.Input;
using UnityEngine;

namespace Test
{
    public class TestInput : MonoBehaviour
    {
        private ISimpleInput _simpleInput;
        
        private void Awake()
        {
            _simpleInput = InputService();
        }

        [ContextMenu("Print")]
        private void Print()
        {
            Vector2 origin = new Vector2(0, -1f);
            
            Vector3 look = new Vector3(0, 0, -1);
            look = look.normalized;
            
            Debug.Log(look);
        }

        private void Update()
        {
            Debug.Log($"Look: {_simpleInput.LookAxis}");
        }
        
        private ISimpleInput InputService()
        {
#if REWIRED_INPUT
        return new RewiredInput();
#else
            return new UnityInput(null);
#endif
        }
    }
}