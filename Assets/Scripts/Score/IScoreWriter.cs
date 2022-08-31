namespace Score
{
    public interface IScoreWriter
    {
        void Accrue(int score);
    }
}