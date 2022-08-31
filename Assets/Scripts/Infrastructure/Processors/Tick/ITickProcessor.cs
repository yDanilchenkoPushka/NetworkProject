using Services;

namespace Infrastructure.Processors.Tick
{
    public interface ITickProcessor : IService
    {
        void Add(ITickable tick);
        void Remove(ITickable tick);
    }
}