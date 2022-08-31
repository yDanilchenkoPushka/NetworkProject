using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.Processors.Tick
{
    public class TickProcessor : MonoBehaviour, ITickProcessor
    {
        private List<ITickable> _ticks = new List<ITickable>();

        public void Add(ITickable tick)
        {
            if (_ticks.Contains(tick))
                return;

            _ticks.Add(tick);
        }

        public void Remove(ITickable tick)
        {
            if (IsNotContainsTick(tick))
                return;

            _ticks.Remove(tick);
        }

        private void Update()
        {
            for (int i = 0; i < _ticks.Count; i++)
                _ticks[i].Tick();
        }

        private bool IsNotContainsTick(ITickable tick) =>
            !_ticks.Contains(tick);
    }
}