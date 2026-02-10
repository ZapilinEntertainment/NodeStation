using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ZE.NodeStation
{
    public class TrainsTimetableController : IDisposable, ITickable
    {
        public TimeSpan CurrentTime => _currentTick;

        private readonly LevelConfig _levelConfig;
        private readonly TimeSpan _endTime;
        private readonly List<TimetabledTrain> _trains;
        private readonly TimetabledTrainBuilder _timetabledTrainBuilder;
        private readonly TrainsTimetableWindowController _windowController;

        // TODO: discrete into autonomous TimeManager
        private TimeSpan _currentTick;
        private float _secondsLeft = 0f;
        private bool _isCompleted = false;
       

        [Inject]
        public TrainsTimetableController(LevelConfig levelConfig, TimetabledTrainBuilder timetabledTrainBuilder, TrainsTimetableWindowController windowController)
        {
            _levelConfig = levelConfig;
            _timetabledTrainBuilder = timetabledTrainBuilder;
            _windowController = windowController;

            _currentTick = _levelConfig.EndTime.ToTimeSpan();
            _endTime = _levelConfig.EndTime.ToTimeSpan();

            var trains = levelConfig.Trains;
            var count = trains.Length;
            _trains = new List<TimetabledTrain>(count);

            for (var i = 0; i < count; i++)
            {
                _trains.Add(_timetabledTrainBuilder.Build(trains[i]));
            }
        }

        public void Tick()
        {
            if (_isCompleted)
                return;

            _secondsLeft += Time.deltaTime;
            if (_secondsLeft >= 1f)
            {
                var secondsInt = (int)_secondsLeft;
                _currentTick.Add(new(hours:0, minutes:0, seconds: secondsInt));
                _secondsLeft -= secondsInt;

                if (_currentTick > _endTime)
                {
                    EndLevel();
                }
                else
                {
                    if (_trains.Count != 0)
                    {
                        for (var i = 0; i < _trains.Count; i++)
                        {
                            var train = _trains[i];
                            switch (train.Status)
                            {
                                case TimetabledTrainStatus.NotReady:
                                    {
                                        if (train.LabelAppearTime >= _currentTick)
                                            ShowTrainLabel(train);
                                        break;
                                    }
                                    case TimetabledTrainStatus.Announced:
                                    {
                                        if (train.TrainLaunchTime >= _currentTick)
                                            LaunchTrain(train);
                                        break;
                                    }
                                    case TimetabledTrainStatus.Launched:
                                    {
                                        if (train.IsReachedDestination)
                                            train.Status = TimetabledTrainStatus.CompletedRoute;
                                        break;
                                    }
                                    case TimetabledTrainStatus.CompletedRoute:
                                    {
                                        train.Dispose();
                                        _trains.RemoveAt(i);
                                        i--;
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
        }

        public void Dispose() 
        { 
            foreach (var train in _trains)
            {
                train.Dispose();
                _trains.Clear();
            }           
        }

        // TODO: rework to signal
        private void EndLevel()
        {
            _isCompleted = true;
            Debug.Log("Level completed!");
        }

        private void ShowTrainLabel(TimetabledTrain train)
        {
            train.Status = TimetabledTrainStatus.Announced;
            _windowController.AddLine(train);
        }

        private void LaunchTrain(TimetabledTrain train)
        {
            train.Status = TimetabledTrainStatus.Launched;
        }
    }
}
