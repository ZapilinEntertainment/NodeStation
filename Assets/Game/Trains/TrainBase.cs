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
        public bool IsReachedDestination { get; private set; } = false;

        protected readonly TrainConfiguration Config;
        protected readonly RailMovementCalculator RailMovementCalculator;
        protected readonly PathsMap Map;
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
            RailMovementCalculator = protocol.RailMovementCalculator;
            Map = protocol.Map;
            _tickableManager = protocol.TickableManager;

            Config = config;

            _lifetimeObject = lifetimeObject;
            _lifetimeObject.DisposedEvent += Dispose;
            _tickableManager.Add(this);
        }

        public void Activate() => _mode = TrainActivityMode.Active;

        public virtual void SetPosition(in RailPosition pos)
        {
            _position = pos;
            _isReversed = pos.IsReversed;
        }

        public void SetSpeed(float speedPc, bool isAccelerating)
        {
            _speed = speedPc * Config.MaxSpeed;
            _isAccelerating = isAccelerating;
        }

        public void Tick()
        {
            if (_mode != TrainActivityMode.Active)
                return;

            var dt = Time.deltaTime;
            if (_isAccelerating)
            {
                _speed = Mathf.MoveTowards(_speed, Config.MaxSpeed, dt * Config.Acceleration);
            }
            else
            {
                _speed = Mathf.MoveTowards(_speed, 0f, dt * Config.Deceleration);
            }

            if (_speed != 0f)
                DoMove(dt);
        }

        public void Dispose()
        {
            if (_mode == TrainActivityMode.Disposed)
                return;

            _lifetimeObject.DisposedEvent -= Dispose;
            _tickableManager.Remove(this);

            _mode = TrainActivityMode.Disposed;
            IsReachedDestination = true;
            DisposedEvent?.Invoke();
        }

        protected virtual void DoMove(float deltaTime)
        {
            var movementResult = RailMovementCalculator.MoveNext(_position, new(_speed * deltaTime, _isReversed));
            SetPosition(movementResult.Position);
            switch (movementResult.EventType)
            {
                case PostMovementEventType.Derail: Derail(); return;
                case PostMovementEventType.Disappear: Dispose(); return;
            }
        }

        protected virtual void Derail()
        {
            _mode = TrainActivityMode.Disabled;
        }       
    }
}
