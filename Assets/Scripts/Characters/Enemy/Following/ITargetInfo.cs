using System;

namespace Characters.Enemy.Following
{
    public interface ITargetInfo
    {
        event Action OnTargetUpdated;
        IEnemyTarget Target { get; }
        bool HasTarget { get; }
    }
}