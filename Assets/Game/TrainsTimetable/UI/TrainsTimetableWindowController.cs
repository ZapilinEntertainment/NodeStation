using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UniRx;

namespace ZE.NodeStation
{
    public class TrainsTimetableWindowController : IDisposable
    {
        public struct SelectionData
        {
            public IRoute Route;
            public TimetabledTrain Train;

            public bool IsEmpty => Route == null || Train == null;
        }

        public IReadOnlyReactiveProperty<SelectionData> SelectionProperty => _selectedProperty;

        private readonly TrainsTimetableWindow _window;
        private readonly RouteDrawManager _routeDrawManager;
        private readonly RoutesManager _routesManager;
        private readonly TimeManager _timeManager;
        private readonly IGUIColorsPalette _guiColorsPalette;

        private readonly TrainRouteHighlightWindowController _routeHighlightWindowController;
        private readonly Dictionary<TimetabledTrain, TrainTimetableLine> _lines = new();        
        private readonly ReactiveProperty<SelectionData> _selectedProperty = new();

        [Inject]
        public TrainsTimetableWindowController(
            TrainsTimetableWindow window, 
            RouteDrawManager routeDrawManager, 
            RoutesManager routesManager,
            TimeManager timeManager,
            IGUIColorsPalette guiColorsPalette,
            ISceneFlagsManager sceneFlags)
        {
            _window = window;
            _routeDrawManager = routeDrawManager;
            _routesManager = routesManager;
            _timeManager = timeManager;
            _guiColorsPalette = guiColorsPalette;

            _routeHighlightWindowController = new TrainRouteHighlightWindowController(_window.RouteHighlightWindow, _timeManager, _guiColorsPalette, sceneFlags);
            _routeHighlightWindowController.Init(_selectedProperty, StopHighlighting);
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

            var isLineSelectedObservable = _selectedProperty
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

            if (train == _selectedProperty.Value.Train)
                StopHighlighting();
        }

        private void OnTrainLineClicked(TimetabledTrain train)
        {
            if (_routesManager.TryGetRoute(train, out var route))
            {
                if (!_selectedProperty.Value.IsEmpty)
                    _routeDrawManager.ClearRouteDrawing(_selectedProperty.Value.Route);
                _routeDrawManager.DrawRoute(route);
                _selectedProperty.Value = new() { Route = route, Train = train };
            }                
        }

        private void StopHighlighting()
        {
            if (_selectedProperty.Value.IsEmpty)
                return;

            _routeDrawManager.ClearRouteDrawing(_selectedProperty.Value.Route);
            _selectedProperty.Value = default;
        }
    }
}
