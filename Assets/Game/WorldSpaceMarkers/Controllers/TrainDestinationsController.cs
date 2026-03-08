using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;

namespace ZE.NodeStation
{
    public class TrainDestinationsController : IStartable, IDisposable
    {
        private readonly LevelConfig _levelConfig;
        private readonly WorldSpaceMarkersFactory _markersFactory;
        private readonly PathsMap _map;
        private readonly ISceneFlagsManager _sceneFlags;

        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly List<WorldSpaceMarker> _destinationMarkers = new();

        [Inject]
        public TrainDestinationsController(
            LevelConfig levelConfig, 
            WorldSpaceMarkersFactory markersFactory, 
            PathsMap map,
            ISceneFlagsManager sceneFlags)
        {
            _levelConfig = levelConfig;
            _markersFactory = markersFactory;
            _map = map;
            _sceneFlags = sceneFlags;
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
                _destinationMarkers.Add(marker);
            }

            // TODO: check which destination is target and which ones can be reached
            _sceneFlags
                .Subscribe<TrainRouteHighlightFlag>(isActive => SetMarkersVisibility(!isActive))
                .AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
            _destinationMarkers.Clear();
        }    
        
        private void SetMarkersVisibility(bool isVisible)
        {
            foreach (var marker in _destinationMarkers) 
                marker.SetActivity(isVisible);
        }
    }
}
