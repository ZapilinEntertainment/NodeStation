using System;
using UnityEngine;
using UniRx;

namespace ZE.NodeStation
{
    public class TrainRouteHighlightWindowController : IDisposable
    {
        private readonly RouteHighlightWindow _window;
        private readonly TimeManager _timeManager;
        private readonly ISceneFlagsManager _flags;
        private readonly IGUIColorsPalette _guiColors;

        private CompositeDisposable _compositeDisposable = new();
        private TrainRouteHighlightFlag _routeHighlightFlag;

        public TrainRouteHighlightWindowController(
            RouteHighlightWindow window, 
            TimeManager timeManager, 
            IGUIColorsPalette guiColors,
            ISceneFlagsManager flags)
        {
            _window = window;
            _timeManager = timeManager;
            _guiColors = guiColors;
            _flags = flags;
        }
        
        public void Init(IObservable<TrainsTimetableWindowController.SelectionData> highlightProperty, Action cancelAction)
        {
            var existingRouteData = highlightProperty.Where(data => !data.IsEmpty);

            var observableArrivalLabel = 
                existingRouteData
                .CombineLatest(
                    _timeManager.CurrentTimeProperty, 
                    (TrainsTimetableWindowController.SelectionData data, TimeSpan currentTime) => 
                    {
                        var launchTime = data.Train.TrainLaunchTime;
                        if (currentTime < launchTime) 
                        { 
                            var deltaTime = data.Train.TrainLaunchTime - currentTime;
                            return $"Arrival in {deltaTime.Hours:D2}:{deltaTime.Minutes:D2}";
                        }
                        else
                        {
                            return "Arrived";
                        }
                    })
                .DistinctUntilChanged();

            var observableRouteData = existingRouteData
                .Select(data => new RouteHighlightWindow.SelectedRouteData()
                {
                    Color = _guiColors.GetGUIColor(data.Route.ColorKey),
                    ObservableArrivalLabel = observableArrivalLabel,
                    RouteLabel = data.Train.RouteText,
                    ObservableIsRouteCorrect = data.Route.StatusProperty.Select(status => status == RouteStatus.Correct),
                });

            highlightProperty
                .Select(data => data.IsEmpty)
                .DistinctUntilChanged()
                .Subscribe(isDataEmpty =>
                {
                    if (isDataEmpty)
                    {
                        _window.Hide();
                    }                        
                    else
                    {
                        _window.Setup(observableRouteData, cancelAction);
                        _window.Show();
                    }                        
                })
                .AddTo(_compositeDisposable);

            highlightProperty
                .Subscribe(data =>
                {
                    if (_routeHighlightFlag != null)
                        _flags.RemoveFlag(_routeHighlightFlag);

                    if (!data.IsEmpty)
                    {
                        _routeHighlightFlag = new(data.Train, data.Route);
                        _flags.AddFlag(_routeHighlightFlag);
                    }
                })
                .AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            if (_flags != null && _routeHighlightFlag != null)
                _flags.RemoveFlag(_routeHighlightFlag);

            _compositeDisposable.Dispose();
            if (_window != null)
                _window.Hide();
        }
    }
}
