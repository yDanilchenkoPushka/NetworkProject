using UnityEngine;

namespace Characters.Enemy.Patrol
{
    public interface IPath
    {
        Vector3 NextPosition(LoopMode mode);
    }
}