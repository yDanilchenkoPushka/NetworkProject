using System;

namespace Score
{
    public interface IScoreReader
    {
        event Action<int> OnScoreUpdated;
    }
}