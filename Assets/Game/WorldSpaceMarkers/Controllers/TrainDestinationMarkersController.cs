using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;

namespace ZE.NodeStation
{
    public class TrainDestinationMarkersController : IStartable, IDisposable
    {
        private readonly LevelConfig _levelConfig;
        private readonly WorldSpaceMarkersFactory _markersFactory;
        private readonly PathsMap _map;
        private readonly RoutesManager _routesManager;
        private readonly IRouteHighlighlightController _routeHighlighter;
        private readonly IMessageBroker _messageBroker;
        private readonly IGUIColorsPalette _guiColors;

        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly Dictionary<int, TrainDestinationMarker> _destinationMarkers = new();

        private IRoute _highlightedRoute;

        [Inject]
        public TrainDestinationMarkersController(
            LevelConfig levelConfig, 
            WorldSpaceMarkersFactory markersFactory, 
            PathsMap map,
            RoutesManager routesManager,
            IRouteHighlighlightController routeHighlighlightController,
            IMessageBroker messageBroker,
            IGUIColorsPalette guiColors)
        {
            _levelConfig = levelConfig;
            _markersFactory = markersFactory;
            _map = map;
            _routesManager = routesManager;

            _routeHighlighter = routeHighlighlightController;
            _messageBroker = messageBroker;
            _guiColors = guiColors;
        }

        public void Start()
        {
            var markerPoints = GameObject.FindObjectsByType<DestinationLabelMarkerPoint>(FindObjectsSortMode.None);
            if (markerPoints.Length == 0)
            {
                Debug.LogWarning("no destination markers found!");
                Dispose();
                return;
            }

            foreach (var markerPoint in markerPoints)
            {
                // todo: also count marker GO direction
                var destination = _levelConfig.Destinations[markerPoint.DestinationIndex];
                var marker = _markersFactory.CreateTrainDestinationMarker(markerPoint.transform.position);
                marker.Setup(destination.NameKey);
                _compositeDisposable.Add(marker);
                _destinationMarkers.Add(destination.NodeKey, marker);
            }

            _routeHighlighter.ObservableHighlightedRoute
                .Subscribe(UpdateMarkersVisibility)
                .AddTo(_compositeDisposable);

            // note: use cached value, not direct from source (can run into outdated value otherwise)
            _messageBroker
                .Receive<RouteChangedMessage>()
                .Subscribe(_ => UpdateMarkersVisibility(_highlightedRoute))
                .AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
            _destinationMarkers.Clear();
        }    

        private void UpdateMarkersVisibility(IRoute highlightedRoute)
        {
            _highlightedRoute = highlightedRoute;
            if (_highlightedRoute != null) 
            { 
                var targetNodeKey = Constants.NO_EXIT_PATH_CODE;
                if ( _routesManager.TryGetRouteController(_highlightedRoute, out var routeController)
                    && routeController.CurrentExitNode != null)
                    targetNodeKey = routeController.CurrentExitNode.Key;

                foreach (var markerKvp in _destinationMarkers)
                {
                    if ( markerKvp.Key == targetNodeKey)
                    {
                        var marker = markerKvp.Value;

                        if (_highlightedRoute.Status == RouteStatus.Correct)
                            marker.SetColor(_guiColors.GetGUIColor(_highlightedRoute.ColorKey));
                        else 
                            marker.ResetColor();
                        marker.SetActivity(true);
                    }
                    else
                    {
                        markerKvp.Value.SetActivity(false);
                    }
                }
            }
            else
            {
                foreach (var markerKvp in _destinationMarkers)
                {
                    markerKvp.Value.ResetColor();
                    markerKvp.Value.SetActivity(true);                   
                }
            }
        }
    }
}
