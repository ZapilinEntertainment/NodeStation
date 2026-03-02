using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UniRx;

namespace ZE.NodeStation
{
    public class TrainsTimetableWindowController : IDisposable
    {
        private readonly TrainsTimetableWindow _window;
        private readonly RouteDrawManager _routeDrawManager;
        private readonly RoutesManager _routesManager;
        private readonly TimeManager _timeManager;
        private readonly Dictionary<TimetabledTrain, TrainTimetableLine> _lines = new();
        private readonly IGUIColorsPalette _guiColorsPalette;
        private readonly ReactiveProperty<IRoute> _highlightingRouteProperty = new();

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

            var isLineSelectedObservable = _highlightingRouteProperty
                .Select( currentRoute => currentRoute == route );

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

            _highlightingRouteProperty.Dispose();
        }

        private void OnTrainDisposed(TimetabledTrain train)
        {
            if (_lines.TryGetValue(train, out var line))
            {
                line?.Dispose();
                _lines.Remove(train);
            }
        }

        private void OnTrainLineClicked(TimetabledTrain train)
        {
            if (_routesManager.TryGetRoute(train, out var route))
            {
                if (_highlightingRouteProperty.Value != null)
                    _routeDrawManager.ClearRouteDrawing(_highlightingRouteProperty.Value);
                _routeDrawManager.DrawRoute(route);
                _highlightingRouteProperty.Value = route;
            }                
        }
    }
}
