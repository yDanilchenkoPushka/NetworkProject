using UnityEngine;

namespace Characters.Enemy.Following
{
    public interface IEnemyTarget
    {
        Vector3 Position { get; }
    }
}