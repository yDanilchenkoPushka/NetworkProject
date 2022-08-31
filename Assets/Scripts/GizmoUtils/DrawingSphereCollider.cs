using UnityEngine;

namespace GizmoUtils
{
    [RequireComponent(typeof(SphereCollider))]
    public class DrawingSphereCollider : MonoBehaviour
    {
        [SerializeField]
        private Color _color = Color.magenta;
        
        [SerializeField, HideInInspector]
        private SphereCollider _sphereCollider;

        private void OnValidate() => 
            _sphereCollider = GetComponent<SphereCollider>();
        
        private void OnDrawGizmos()
        {
            Gizmos.color = _color;

            Gizmos.DrawSphere(transform.position, _sphereCollider.radius);
        }
    }
}