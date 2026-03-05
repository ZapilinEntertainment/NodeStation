using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;

namespace ZE.NodeStation
{
    public class TrainsTimetableWindowController : IDisposable, IStartable
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
        private readonly IMessageBroker _messageBroker;
        private readonly ITimetabledTrainsList _trainsList;

        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly TrainRouteHighlightWindowController _routeHighlightWindowController;
        private readonly Dictionary<TimetabledTrain, int> _activeLines = new();        
        private readonly ReactiveProperty<SelectionData> _selectedProperty = new();
        private readonly TrainTimetableLine[] _linesInOrder = new TrainTimetableLine[Constants.MAX_ROUTE_BUTTONS];

        private int _nextButtonIndex = 0;

        [Inject]
        public TrainsTimetableWindowController(
            TrainsTimetableWindow window, 
            RouteDrawManager routeDrawManager, 
            RoutesManager routesManager,
            TimeManager timeManager,
            IGUIColorsPalette guiColorsPalette,
            ISceneFlagsManager sceneFlags,
            IMessageBroker messageBroker,
            ITimetabledTrainsList trainsList)
        {
            _window = window;
            _routeDrawManager = routeDrawManager;
            _routesManager = routesManager;
            _timeManager = timeManager;
            _guiColorsPalette = guiColorsPalette;
            _messageBroker = messageBroker;
            _trainsList = trainsList;

            _routeHighlightWindowController = new TrainRouteHighlightWindowController(_window.RouteHighlightWindow, _timeManager, _guiColorsPalette, sceneFlags);
            _routeHighlightWindowController.Init(_selectedProperty, StopHighlighting);
        }

        public void Start()
        {
            var trains = _trainsList.Trains;
            var count = Mathf.Min(trains.Count, Constants.MAX_ROUTE_BUTTONS);
            var j = 0;
            for (var i = 0; i< count;i++ )
            {
                var train = trains[i];
                if (train.Status.CanChangeRoute())
                {
                    AddRouteButton(trains[i]);
                    j++;
                }
                    
            }

            if (j < Constants.MAX_ROUTE_BUTTONS)
            {
                for (; j < Constants.MAX_ROUTE_BUTTONS; j++) 
                {
                    GetLineAt(j).SwitchToDisabled();
                }
            }

            _messageBroker
                .Receive<TrainAnnouncedMessage>()
                .Select(msg => msg.Train)
                .Subscribe(train => AddRouteButton(train))
                .AddTo(_compositeDisposable);
        }

        private void AddRouteButton(TimetabledTrain train)
        {
            if (!_routesManager.TryGetRoute(train, out var route))
                return;

            if (_activeLines.Count == Constants.MAX_ROUTE_BUTTONS)
            {
                Debug.LogError("too much tracking routes!");
                return;                
            }

            var line = GetLineAt(_nextButtonIndex);

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
            _activeLines.Add(train, _nextButtonIndex);    
            train.DisposeEvent += () => OnTrainDisposed(train);

            _linesInOrder[_nextButtonIndex] = line;

            for (var i = 0; i < _linesInOrder.Length; i++)
            {
                _nextButtonIndex = (_nextButtonIndex + 1) % Constants.MAX_ROUTE_BUTTONS;
                if (!_activeLines.ContainsValue(_nextButtonIndex))
                    break;
            }
        }       

        public void Dispose()
        {
            foreach (var trainLine in _linesInOrder)
            {
                trainLine?.Dispose();
            }
            _activeLines.Clear();

            _routeHighlightWindowController.Dispose();
            _compositeDisposable.Dispose();
        }

        private TrainTimetableLine GetLineAt(int index)
        {
            var line = _linesInOrder[index];
            if (line != null)
                return line;

            line = _window.GetOrCreateLinesPool().Get();
            _linesInOrder[index] = line;
            return line;
        }

        private void OnTrainDisposed(TimetabledTrain train)
        {
            if (_activeLines.TryGetValue(train, out var lineIndex))
            {
                GetLineAt(lineIndex).SwitchToDisabled();
                _activeLines.Remove(train);
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
