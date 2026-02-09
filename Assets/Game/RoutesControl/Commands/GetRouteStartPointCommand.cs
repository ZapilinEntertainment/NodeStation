using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class GetRouteStartPointCommand
    {
        private readonly PathsMap _map;

        [Inject]
        public GetRouteStartPointCommand(PathsMap map)
        {
            _map = map;
        }

        public RailPosition Execute(TrainRoute route)
        {
            var points = route.Points;
            var point0 = points[0];
            var point1 = points[1];

            var pathKey = new PathKey(point0, point1);
            if (!_map.TryGetPath(pathKey, out var path))
            {
                Debug.LogError("Route start segment invalid");
                return default;
            }

            var isReversed = path.PathKey.StartNodeKey == point1;
            if (isReversed)
                return path.GetPosition(1f, true);
            else
                return path.GetPosition(0f, false);
        }
    
    }
}
