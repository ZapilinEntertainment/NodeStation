using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;

namespace ZE.NodeStation
{
    public class TrainsTimetableController : IDisposable
    {
        private readonly LevelConfig _levelConfig;
        private readonly List<TimetabledTrain> _trains;
        private readonly TimetabledTrainBuilder _timetabledTrainBuilder;
        private readonly TrainsTimetableWindowController _windowController;
        private readonly LaunchTimetabledTrainCommand _launchTrainCommand;
        private readonly IMessageBroker _messageBroker;
        private readonly CompositeDisposable _compositeDisposable = new();

        [Inject]
        public TrainsTimetableController(
            LevelConfig levelConfig, 
            TimetabledTrainBuilder timetabledTrainBuilder, 
            TrainsTimetableWindowController windowController,
            TimeManager timeManager,
            LaunchTimetabledTrainCommand launchTrainCommand,
            IMessageBroker messageBroker)
        {
            _levelConfig = levelConfig;
            _timetabledTrainBuilder = timetabledTrainBuilder;
            _windowController = windowController;
            _launchTrainCommand = launchTrainCommand;
            _messageBroker = messageBroker;

            var trains = levelConfig.Trains;
            var count = trains.Length;
            _trains = new List<TimetabledTrain>(count);

            var time = levelConfig.StartTime.ToTimeSpan();
            for (var i = 0; i < count; i++)
            {
                var timetabledTrain = _timetabledTrainBuilder.Build(trains[i]);
                _trains.Add(timetabledTrain);
                time = timetabledTrain.TrainLaunchTime;
            }

            timeManager.CurrentTimeProperty.Subscribe(OnTimeChanged).AddTo(_compositeDisposable);
            //TEST
            timeManager.IsShiftEndedProperty
                .Where(x => x == true)
                .Subscribe(_ => Debug.Log("level completed!"))
                .AddTo(_compositeDisposable);
        }

        private void OnTimeChanged(TimeSpan time)
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
                                if (train.LabelAppearTime <= time)
                                    ShowTrainLabel(train);
                                break;
                            }
                        case TimetabledTrainStatus.Announced:
                            {
                                if (train.TrainLaunchTime <= time)
                                    _launchTrainCommand.Execute(train);
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

        public void Dispose() 
        { 
            _compositeDisposable.Dispose();
            foreach (var train in _trains)
            {
                train.Dispose();               
            }
            _trains.Clear();
        }

        private void ShowTrainLabel(TimetabledTrain train)
        {            
            train.Status = TimetabledTrainStatus.Announced;
            _windowController.AddLine(train);
        }
    }
}
