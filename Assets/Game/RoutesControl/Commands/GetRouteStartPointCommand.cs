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

        public RailPosition Execute(IRoute route, float percent, bool isReversed)
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

            return path.GetPosition(percent, isReversed);
        }

        public RailPosition Execute(int spawnNodeKey)
        {
            if (!_map.TryGetNode(spawnNodeKey, out var spawnNode))
            {
                Debug.LogError($"route spawn node not defined: {spawnNodeKey}");
                return default;
            }

            if (!spawnNode.TryGetExitNode(Constants.NO_EXIT_PATH_CODE, out var exitNodeKey))
            {
                Debug.LogError($"route spawn exit node invalid: {exitNodeKey}");
                return default;
            }

            var pathKey = new PathKey(spawnNodeKey, exitNodeKey);
            //Debug.Log($"{pathKey.StartNodeKey} -> {pathKey.EndNodeKey}");
            if (!_map.TryGetPath(pathKey, out var path))
            {
                Debug.LogError("Route start segment invalid");
                return default;
            }

            var isReversed = path.PathKey.StartNodeKey == exitNodeKey;
            return path.GetPosition(isReversed ? 1f: 0f, isReversed);
        }

    }
}
