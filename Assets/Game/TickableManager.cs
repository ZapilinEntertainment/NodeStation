using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ZE.NodeStation
{
    public class TickableManager : IDisposable, ITickable
    {
        private readonly HashSet<ITickable> _tickables = new();
        private readonly HashSet<ITickable> _clearList = new();

        public void Add(ITickable tickable) => _tickables.Add(tickable);
        public void Remove(ITickable tickable) => _clearList.Add(tickable);

        public void Tick()
        {
            if (_clearList.Count != 0)
            {
                foreach (var tickable in _clearList)
                {
                    _tickables.Remove(tickable);
                }
                _clearList.Clear();
            }

            foreach (var tickable in _tickables)
            {
                tickable.Tick();
            }
        }

        public void Dispose()
        {
            _tickables.Clear();
        }        
    }
}
