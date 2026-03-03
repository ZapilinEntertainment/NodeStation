using System;
using UnityEngine;
using UniRx;

namespace ZE.NodeStation
{
    public class RouteHighlightWindowController : IDisposable
    {
        private readonly RouteHighlightWindow _window;
        private readonly TimeManager _timeManager;
        private readonly IGUIColorsPalette _guiColors;
        private IDisposable _subscription;

        public RouteHighlightWindowController(RouteHighlightWindow window, TimeManager timeManager, IGUIColorsPalette guiColors)
        {
            _window = window;
            _timeManager = timeManager;
            _guiColors = guiColors;
        }

        public void Init(IReadOnlyReactiveProperty<TrainsTimetableWindowController.HighlightedData> highlightProperty, Action cancelAction)
        {
            var existingRouteData = highlightProperty.Where(data => !data.IsEmpty);

            var observableArrivalLabel = 
                existingRouteData
                .CombineLatest(
                    _timeManager.CurrentTimeProperty, 
                    (TrainsTimetableWindowController.HighlightedData data, TimeSpan currentTime) => 
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
                    RouteLabel = data.Train.RouteText
                });

            _subscription = highlightProperty
                .Select(data => data.IsEmpty)
                .DistinctUntilChanged()
                .Subscribe(isDataEmpty =>
                {
                    if (isDataEmpty)
                        _window.Hide();
                    else
                        _window.Show();
                });

            _window.Setup(observableRouteData, cancelAction);
        }

        public void Dispose()
        {
            _subscription.Dispose();
            if (_window != null)
                _window.Hide();
        }
    }
}
