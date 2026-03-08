using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public abstract class WorldSpaceMarker : IDisposable, ILateFrameTickable
    {
        protected bool _isActive = true;
        protected event Action DisposeEvent;
        protected readonly Func<Vector3, Vector2> ConversionFunc;
        protected readonly IWorldMarkerUiView View;
        private readonly TickableManager _tickableManager;

        protected abstract Vector3 WorldPos { get; }

        public WorldSpaceMarker(Func<Vector3, Vector2> conversionFunc, IWorldMarkerUiView view, TickableManager tickableManager)
        {
            ConversionFunc = conversionFunc;
            View = view;
            _tickableManager = tickableManager;

            _tickableManager.Add(this);
        }

        public void Tick()
        {
            if (!_isActive) return;

            var screenPos = ConversionFunc(WorldPos);
            View.SetPosition(screenPos);
        }

        public void Dispose()
        {
            _tickableManager?.Remove(this);
            View.Dispose();
            DisposeEvent?.Invoke();
            DisposeEvent = null;
        }       

        public void SetActivity(bool x)
        {
            _isActive = x;
            View.SetVisible(x);
        }
    }
}
