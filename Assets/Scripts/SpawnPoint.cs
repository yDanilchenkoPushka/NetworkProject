using System;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Vector3 Position => transform.position;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}