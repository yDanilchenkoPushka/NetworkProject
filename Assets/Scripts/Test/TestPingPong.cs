using UnityEngine;

namespace Test
{
    public class TestPingPong : MonoBehaviour
    {
        [ContextMenu("Log")]
        public void Log()
        {
            int length = 7;
            int current = 0;

           
            
            for (int i = 0; i < 100; i++)
            {
                current = (int)Mathf.PingPong(i, length - 1);
                
                Debug.Log("Current: " + current);
            }
        }
    }
}