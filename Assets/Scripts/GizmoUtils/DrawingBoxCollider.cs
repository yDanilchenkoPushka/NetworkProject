using UnityEngine;

namespace GizmoUtils
{
    [RequireComponent(typeof(BoxCollider))]
    public class DrawingBoxCollider : MonoBehaviour
    {
        [SerializeField]
        private Color _color = Color.magenta;
        
        [SerializeField, HideInInspector]
        private BoxCollider _boxCollider;

        private void OnValidate() => 
            _boxCollider = GetComponent<BoxCollider>();
        
        private void OnDrawGizmos()
        {
            Gizmos.color = _color;

            Vector3 size = new Vector3(
                _boxCollider.size.x * transform.localScale.x,
                _boxCollider.size.y * transform.localScale.y,
                _boxCollider.size.z * transform.localScale.z);
            
            Gizmos.DrawCube(transform.position, size);
        }
    }
}