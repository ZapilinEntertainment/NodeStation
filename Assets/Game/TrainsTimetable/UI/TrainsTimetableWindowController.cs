using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class TrainsTimetableWindowController : IDisposable
    {
        private readonly TrainsTimetableWindow _window;
        private readonly RouteDrawManager _routeDrawManager;
        private readonly RoutesManager _routesManager;
        private readonly Dictionary<TimetabledTrain, TrainTimetableLine> _lines = new();
        private IRoute _currentVisibleRoute;

        [Inject]
        public TrainsTimetableWindowController(TrainsTimetableWindow window, RouteDrawManager routeDrawManager, RoutesManager routesManager)
        {
            _window = window;
            _routeDrawManager = routeDrawManager;
            _routesManager = routesManager;
        }

        public void AddLine(TimetabledTrain train)
        {
            var line = _window.GetOrCreateLinesPool().Get();

            var appearTime = train.TrainLaunchTime;
            var timeLabel = $"d:{appearTime.Days:D1} {appearTime.Hours:D2}:{appearTime.Minutes:D2}";

            line.Setup(new()
            {
                RouteLabel = train.LabelText,
                TimeLabel = timeLabel,
                StatusProperty = train.StatusProperty,
                OnClickAction = () => OnTrainLineClicked(train)
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
        }

        private void OnTrainDisposed(TimetabledTrain train)
        {
            if (_lines.TryGetValue(train, out var line))
            {
                line.Dispose();
                _lines.Remove(train);
            }
        }

        private void OnTrainLineClicked(TimetabledTrain train)
        {
            if (_routesManager.TryGetRoute(train, out var route))
            {
                if (_currentVisibleRoute != null)
                    _routeDrawManager.ClearRouteDrawing(_currentVisibleRoute);
                _routeDrawManager.DrawRoute(route);
                _currentVisibleRoute = route;
            }                
        }
    }
}
