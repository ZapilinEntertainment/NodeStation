using System;
using UnityEngine;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace ZE.NodeStation
{
    public class TimeManager : IDisposable, ITickable
    {
        public bool IsShiftEnded => _isShiftEndedProperty.Value;
        public IReactiveProperty<bool> IsShiftEndedProperty => _isShiftEndedProperty;
        public IReadOnlyReactiveProperty<TimeSpan> CurrentTimeProperty => _timeProperty;
        public TimeSpan CurrentTime { get => _timeProperty.Value; private set => _timeProperty.Value = value; }
        public float LevelProgress => (_endTime - CurrentTime).Ticks / (float)_totalTicks; 

        private readonly ReactiveProperty<TimeSpan> _timeProperty = new();
        private readonly BoolReactiveProperty _isShiftEndedProperty = new(false);
        private readonly TimeSpan _endTime;
        private readonly TickableManager _tickableManager;
        private long _totalTicks;

        private float _secondsLeft;

        [Inject]
        public TimeManager(LevelConfig levelConfig, TickableManager tickableManager)
        {
            var startTime = levelConfig.StartTime.ToTimeSpan();
            _timeProperty.Value = startTime;
            var duration = levelConfig.ShiftDuration.ToTimeSpan();
            _endTime = startTime.Add(duration);
            _totalTicks = duration.Ticks;

            _tickableManager = tickableManager;
            _tickableManager.Add(this);
        }

        public void Tick()
        {
            if (IsShiftEnded)
                return;

            _secondsLeft += Time.deltaTime;
            if (_secondsLeft >= 1f)
            {
                var secondsInt = (int)_secondsLeft;
                CurrentTime = CurrentTime.Add(new(hours: 0, minutes: secondsInt, seconds: 0));
                _secondsLeft -= secondsInt;

                _isShiftEndedProperty.Value = CurrentTime > _endTime;
            }
        }

        public void Dispose()
        {
            _tickableManager?.Remove(this);
            _isShiftEndedProperty.Dispose();
            _timeProperty.Dispose();
        }
    }
}
