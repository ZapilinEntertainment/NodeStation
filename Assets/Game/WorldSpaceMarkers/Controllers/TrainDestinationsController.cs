using System;
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
        private readonly CompositeDisposable _compositeDisposable = new();

        [Inject]
        public TrainDestinationsController(LevelConfig levelConfig, WorldSpaceMarkersFactory markersFactory, PathsMap map)
        {
            _levelConfig = levelConfig;
            _markersFactory = markersFactory;
            _map = map;
        }

        public void Start()
        {
            foreach (var destinantionInfo in _levelConfig.Destinations)
            {
                var nodeKey = destinantionInfo.NodeKey;
                if (!_map.TryGetNode(nodeKey, out var node))
                {
                    Debug.LogWarning($"incorrect destination node: {nodeKey}");
                    continue;
                }

                var marker = _markersFactory.CreateTrainDestinationMarker(node.WorldPosition);
                marker.Setup(destinantionInfo.NameKey);
                _compositeDisposable.Add(marker);
            }
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }        
    }
}
