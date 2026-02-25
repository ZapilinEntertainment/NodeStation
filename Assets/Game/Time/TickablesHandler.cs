using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    public class TickablesHandler<T> : IDisposable where T : ICustomTickable
    {
        private readonly HashSet<T> _tickables = new();
        private readonly HashSet<T> _clearList = new();
        private readonly HashSet<T> _addList = new();

        public void Add(T tickable) => _addList.Add(tickable);

        public void Dispose()
        {
            _tickables.Clear();
            _clearList.Clear();
            _addList.Clear();
        }

        public void Remove(T tickable) => _clearList.Add(tickable);

        public void Tick()
        {
            if (_addList.Count != 0)
            {
                foreach (var item in _addList)
                {
                    _tickables.Add(item);
                }
                _addList.Clear();
            }

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
    }
}
