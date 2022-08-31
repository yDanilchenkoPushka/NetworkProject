using UnityEngine;

public interface IPickable
{
    Rigidbody Rigidbody { get; }
    Transform Transform { get; }
    Vector3 Position { get; }
    Collider Collider { get; }
}