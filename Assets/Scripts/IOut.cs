namespace Characters.Player
{
    public interface IOut<out TValue> where TValue : class
    {
        TValue Value { get; }
    }
}