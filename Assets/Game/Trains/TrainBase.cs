using System;
using UnityEngine;
using VContainer.Unity;

namespace ZE.NodeStation
{
    public enum TrainActivityMode : byte { Disabled, Active, Disposed}

    public class TrainBase : ITrain, ITickable, IDisposable
    {
        public record InjectProtocol
        {
            public readonly RailMovementCalculator RailMovementCalculator;
            public readonly PathsMap Map;
            public readonly TickableManager TickableManager;

            public InjectProtocol(RailMovementCalculator railMovementCalculator, PathsMap map, TickableManager tickableManager)
            {
                RailMovementCalculator = railMovementCalculator;
                Map = map;
                TickableManager = tickableManager;
            }
        }

        public Vector3 WorldPosition => _position.WorldPosition;
        public Quaternion WorldRotation => _position.WorldRotation;
        public event Action DisposedEvent;

        protected readonly TrainConfiguration _config;
        protected readonly RailMovementCalculator _railMovementCalculator;
        protected readonly PathsMap _map;
        private readonly TickableManager _tickableManager;
        private readonly ILifetimeObject _lifetimeObject;

        protected bool _isAccelerating = false;
        protected bool _isReversed = false;
        protected bool _isStopped = false;
        protected float _speed = 0f;
        protected RailPosition _position;
        protected TrainActivityMode _mode;

        public TrainBase(
            InjectProtocol protocol,
            TrainConfiguration config,
            ILifetimeObject lifetimeObject)
        {
            _railMovementCalculator = protocol.RailMovementCalculator;
            _map = protocol.Map;
            _tickableManager = protocol.TickableManager;

            _config = config;

            _lifetimeObject = lifetimeObject;
            _lifetimeObject.DisposedEvent += Dispose;
            _tickableManager.Add(this);
        }

        public void Activate() => _mode = TrainActivityMode.Active;

        public void SetPosition(in RailPosition pos)
        {
            _position = pos;
        }

        public void SetSpeed(float speedPc, bool isAccelerating)
        {
            _speed = speedPc * _config.MaxSpeed;
            _isAccelerating = isAccelerating;
        }

        public void Tick()
        {
            if (_mode != TrainActivityMode.Active)
                return;

            var dt = Time.deltaTime;
            if (_isAccelerating)
            {
                _speed = Mathf.MoveTowards(_speed, _config.MaxSpeed, dt * _config.Acceleration);
            }
            else
            {
                _speed = Mathf.MoveTowards(_speed, 0f, dt * _config.Deceleration);
            }

            if (_speed != 0f)
            {
                var movementResult = _railMovementCalculator.MoveNext(_position, new(_speed * dt, _isReversed));
                SetPosition(movementResult.Position);
                switch (movementResult.EventType)
                {
                    case PostMovementEventType.Derail: Derail(); return;
                    case PostMovementEventType.Disappear: Dispose(); return;
                }
            }
        }

        public void Dispose()
        {
            if (_mode == TrainActivityMode.Disposed)
                return;

            _lifetimeObject.DisposedEvent -= Dispose;
            _tickableManager.Remove(this);

            _mode = TrainActivityMode.Disposed;
            DisposedEvent?.Invoke();
        }

        private void Derail()
        {
            _mode = TrainActivityMode.Disabled;
        }       
    }
}
