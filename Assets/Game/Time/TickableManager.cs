using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;

namespace ZE.NodeStation
{
    // why custom one: VContainer tickable doesn't support manual add/remove
    public class TickableManager : IDisposable, ITickable, IFixedTickable
    {
        private readonly TickablesHandler<IFrameTickable> _regularTickables = new();
        private readonly TickablesHandler<IFixedFrameTickable> _fixedTickables = new();

        public void Dispose()
        {
            _regularTickables.Dispose();
            _fixedTickables.Dispose();
        }

        public void FixedTick()
        {
            _fixedTickables.Tick();
        }

        public void Tick()
        {
            _regularTickables.Tick();
        }

        public void Add(IFrameTickable tickable) => _regularTickables.Add(tickable);
        public void Add(IFixedFrameTickable tickable) => _fixedTickables.Add(tickable);
        public IDisposable AddAsSubscription(IFixedFrameTickable tickable)
        {
            _fixedTickables.Add(tickable);
            return Disposable.Create(() => _fixedTickables?.Remove(tickable));
        }

        public void Remove(IFrameTickable tickable) => _regularTickables.Remove(tickable);
        public void Remove(IFixedFrameTickable tickable) => _fixedTickables.Remove(tickable);
    }
}
