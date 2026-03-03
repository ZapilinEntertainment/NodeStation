using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UniRx;

namespace ZE.NodeStation
{
    public class TrainsTimetableWindowController : IDisposable
    {
        public struct HighlightedData
        {
            public IRoute Route;
            public TimetabledTrain Train;

            public bool IsEmpty => Route == null || Train == null;
        }

        private readonly TrainsTimetableWindow _window;
        private readonly RouteDrawManager _routeDrawManager;
        private readonly RoutesManager _routesManager;
        private readonly TimeManager _timeManager;
        private readonly IGUIColorsPalette _guiColorsPalette;

        private readonly RouteHighlightWindowController _routeHighlightWindowController;
        private readonly Dictionary<TimetabledTrain, TrainTimetableLine> _lines = new();        
        private readonly ReactiveProperty<HighlightedData> _highlightingProperty = new();

        [Inject]
        public TrainsTimetableWindowController(
            TrainsTimetableWindow window, 
            RouteDrawManager routeDrawManager, 
            RoutesManager routesManager,
            TimeManager timeManager,
            IGUIColorsPalette guiColorsPalette)
        {
            _window = window;
            _routeDrawManager = routeDrawManager;
            _routesManager = routesManager;
            _timeManager = timeManager;
            _guiColorsPalette = guiColorsPalette;

            _routeHighlightWindowController = new RouteHighlightWindowController(_window.RouteHighlightWindow, _timeManager, _guiColorsPalette);
            _routeHighlightWindowController.Init(_highlightingProperty, StopHighlighting);
        }

        public void AddLine(TimetabledTrain train)
        {
            if (!_routesManager.TryGetRoute(train, out var route))
                return;

            var line = _window.GetOrCreateLinesPool().Get();

            var appearTime = train.TrainLaunchTime;
            float periodTicks = (appearTime - _timeManager.CurrentTime).Ticks;
            var timeLabel = $"d:{appearTime.Days:D1} {appearTime.Hours:D2}:{appearTime.Minutes:D2}";
            var bgColor =  _guiColorsPalette.GetGUIColor(route.ColorKey);

            var arrivalProgress = _timeManager
                .CurrentTimeProperty
                .Select(time =>
                {
                    var delta = (appearTime - time).Ticks;
                    return Mathf.Clamp01(1f - delta / periodTicks);
                });

            var isLineSelectedObservable = _highlightingProperty
                .Select( data => data.Route == route );

            line.Setup(new()
            {
                Color = bgColor,
                RouteLabel = train.RouteText,
                StatusProperty = train.StatusProperty,
                OnClickAction = () => OnTrainLineClicked(train),
                ArrivalProgressObservable = arrivalProgress,
                IsLineSelectedObservable = isLineSelectedObservable
            });
            _lines.Add(train, line);    
            train.DisposeEvent += () => OnTrainDisposed(train);
        }

        public void Dispose()
        {
            if (_lines.Count != 0)
            {
                foreach (var trainLine in _lines.Values)
                {
                    trainLine.Dispose();
                }
                _lines.Clear();
            }

            _routeHighlightWindowController.Dispose();
        }

        private void OnTrainDisposed(TimetabledTrain train)
        {
            if (_lines.TryGetValue(train, out var line))
            {
                line?.Dispose();
                _lines.Remove(train);
            }

            if (train == _highlightingProperty.Value.Train)
                StopHighlighting();
        }

        private void OnTrainLineClicked(TimetabledTrain train)
        {
            if (_routesManager.TryGetRoute(train, out var route))
            {
                if (!_highlightingProperty.Value.IsEmpty)
                    _routeDrawManager.ClearRouteDrawing(_highlightingProperty.Value.Route);
                _routeDrawManager.DrawRoute(route);
                _highlightingProperty.Value = new() { Route = route, Train = train };
            }                
        }

        private void StopHighlighting()
        {
            if (_highlightingProperty.Value.IsEmpty)
                return;

            _routeDrawManager.ClearRouteDrawing(_highlightingProperty.Value.Route);
            _highlightingProperty.Value = default;
        }
    }
}
