using System;
using Unity.Netcode;

namespace Score
{
    public interface IScoreReader
    {
        NetworkVariable<int> CurrentScore { get; }
    }
}