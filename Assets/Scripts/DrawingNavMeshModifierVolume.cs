using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshModifierVolume))]
public class DrawingNavMeshModifierVolume : MonoBehaviour
{
    [SerializeField] 
    private Color _color = Color.magenta;
        
    [SerializeField]
    private Vector3 _size = Vector3.one;

    [SerializeField, HideInInspector]
    private NavMeshModifierVolume _navMeshModifierVolume;

    private void OnValidate() => 
        _navMeshModifierVolume = GetComponent<NavMeshModifierVolume>();

    private void OnDrawGizmos()
    {
        Gizmos.color = _color;

        Vector3 position = transform.TransformPoint(_navMeshModifierVolume.center);
        Vector3 size = _navMeshModifierVolume.size;

        Gizmos.DrawCube(position, size); 
    }
}