using DG.Tweening;
using UnityEngine;

namespace Characters.Enemy.Patrol
{
    public class PathData : MonoBehaviour, IPath
    {
        [SerializeField]
        private Color _color = Color.magenta;

        [SerializeField]
        private Transform[] _points;

        private int _current;

        public Vector3 NextPosition(LoopMode mode)
        {
            int index;
            
            if (mode == LoopMode.Loop)
            {
                index = _current;
                
                _current = (_current + 1) % _points.Length;
                
                return _points[index].position;
            }
            
            // PingPong
            index = (int)Mathf.PingPong(_current++, _points.Length - 1);
                
            return _points[index].position;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _color;

            for (int i = 0; i < _points.Length - 1; i++) 
                Gizmos.DrawLine(_points[i].position, _points[i + 1].position);
        }
    }
}