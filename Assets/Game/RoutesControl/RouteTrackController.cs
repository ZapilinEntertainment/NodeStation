using System;
using UnityEngine;
using UniRx;

namespace ZE.NodeStation
{
    // todo: rework to message subscription
    public class RouteTrackController : IDisposable
    {
        private readonly ITrain _train;
        private readonly TrainRoute _route;    
        private readonly PathsMap _map;
        private readonly CompositeDisposable _compositeDisposable = new();

        // path where first bogie stands
        private IPathNode _frontPathEndNode;

        public RouteTrackController(ITrain train, TrainRoute route, PathsMap map)
        {
            _train = train;
            _route = route;
            _map = map;           
            
        }

        public void Init()
        {
            _train.RailPositionProperty
                .Subscribe(OnTrainPositionChanged)
                .AddTo(_compositeDisposable);

            _train.DisposedEvent += Dispose;
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }

        private void DefinePathNodes()
        {
            var firstBogiePos = _train.FirstBogiePosition;
            var firstBogiePosPath = firstBogiePos.Rail.PathKey;
            var frontPathEndNodeKey = firstBogiePos.IsReversed ? firstBogiePosPath.StartNodeKey : firstBogiePosPath.EndNodeKey;
            _map.TryGetNode(frontPathEndNodeKey, out _frontPathEndNode);
        }

        private void OnTrainPositionChanged(RailPosition position)
        {
            var firstBogiePos = _train.FirstBogiePosition;
            var firstBogiePosPath = firstBogiePos.Rail.PathKey;
            var frontPathEndNodeKey = firstBogiePos.IsReversed ? firstBogiePosPath.StartNodeKey : firstBogiePosPath.EndNodeKey;
            var frontPathStartNodeKey = firstBogiePos.IsReversed ? firstBogiePosPath.EndNodeKey : firstBogiePosPath.StartNodeKey;
            _map.TryGetNode(frontPathEndNodeKey, out var newFrontPathEndNode);

            if (newFrontPathEndNode != _frontPathEndNode)
            {
                // TODO: remove prev node route markers
                _frontPathEndNode = newFrontPathEndNode;
                // add route marker

                if (_route.TryGetNextPoint(newFrontPathEndNode, out var nextPoint))
                    _frontPathEndNode.TrySetupPath(frontPathStartNodeKey, nextPoint.Key);
            }
        }
    }
}
