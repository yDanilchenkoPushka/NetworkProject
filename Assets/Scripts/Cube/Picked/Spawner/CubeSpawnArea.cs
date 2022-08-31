using UnityEngine;
using Random = UnityEngine.Random;

namespace Cube.Picked.Spawner
{
    public class CubeSpawnArea : MonoBehaviour
    {
        public Vector3 RandomPosition => new Vector3(
            RandomRange(_rect.xMin, _rect.xMax),
            transform.position.y,
            RandomRange(_rect.yMin, _rect.yMax));

        [SerializeField]
        private Vector2 _size = Vector2.one;

        private Rect _rect;

        public void Initialize()
        {
            Vector2 center = new Vector2(
                transform.position.x - _size.x / 2f,
                transform.position.z - _size.y / 2f);
            
            _rect = new Rect(center, _size);
        }

        private float RandomRange(float from, float to) => 
            Random.Range(from, to);

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.4f);

            Vector3 worldSize = new Vector3(_size.x, 0, _size.y);
            
            Gizmos.DrawCube(transform.position, worldSize);
        }
    }
}