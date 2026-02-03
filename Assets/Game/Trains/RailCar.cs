using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public class RailCar : IViewable
    {
        public readonly Bogie FrontBogie;
        public readonly Bogie RearBogie;
        public readonly float BogeysDistance;
        public readonly float CarLength;
        public readonly bool ReverseView;

        public event Action DisposedEvent;
        private bool _isDisposed = false;

        public Vector3 FrontPos => FrontBogie.RailPosition.WorldPosition;
        public Vector3 RearPos => RearBogie.RailPosition.WorldPosition;

        public Vector3 WorldPosition { get;private set; }
        public Quaternion WorldRotation { get; private set; }

        public RailCar(Bogie front, Bogie rear, float carLength, bool reverseView)
        {
            ReverseView = reverseView;

            CarLength = carLength;

            if (!reverseView)
            {
                FrontBogie = front;
                RearBogie = rear;
                BogeysDistance = FrontBogie.Offset + Mathf.Abs(RearBogie.Offset);
            }
            else
            {
                FrontBogie = rear;
                RearBogie = front;
                BogeysDistance = Mathf.Abs(FrontBogie.Offset) + RearBogie.Offset;
            }            
        }

        public void SetPosition(in RailPosition frontPos, in RailPosition rearPos)
        {
            FrontBogie.SetPosition(frontPos);
            RearBogie.SetPosition(rearPos);

            var dir = (FrontPos - RearPos).normalized;
            if (ReverseView) 
                dir *= -1f;

            WorldPosition = FrontPos - dir * FrontBogie.Offset;

            if (dir.sqrMagnitude != 0f) 
                WorldRotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            FrontBogie.Dispose();
            RearBogie?.Dispose();

            _isDisposed = true;
            if (DisposedEvent != null)
            {
                DisposedEvent.Invoke();
                DisposedEvent = null;
            }
        }
    }
}
