using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public class Bogie : IViewable, IDisposable
    {
        public readonly float Offset;
        public RailPosition RailPosition { get; private set; }
        public Vector3 WorldPosition => RailPosition.WorldPosition;
        public Quaternion WorldRotation => RailPosition.WorldRotation;

        public event Action DisposedEvent;

        public Bogie(float offset) => Offset = offset;

        public void SetPosition( in RailPosition position)
        {
            RailPosition = position; 
        }

        public void Dispose()
        {
            if (DisposedEvent != null)
            {
                DisposedEvent.Invoke();
                DisposedEvent = null;
            }
        }
    }
}
