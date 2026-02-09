using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class RouteBuilder
    {
        private readonly PathsMap _map;

        [Inject]
        public RouteBuilder(PathsMap pathsMap)
        {
            _map = pathsMap;
        }

        public bool TryBuildRoute(in RouteSettings routeTargets, out TrainRoute route)
        {
            var points = new List<int>();
            var nodeKey = routeTargets.SpawnNodeKey;
            var nextNodeFound = false;
            var prevNodeKey = Constants.NO_EXIT_PATH_CODE;

            if (!_map.TryGetNode(nodeKey, out var node))
            {
                Debug.LogError("Route spawn node incorrect");
                route = default;
                return false;
            }

            do
            {
                points.Add(nodeKey);

                nextNodeFound = node.TryGetExitNode(prevNodeKey, out var nextNodeKey);
                prevNodeKey = nodeKey;
                nodeKey = nextNodeKey;       

                if (!_map.TryGetNode(nodeKey, out node))
                {
                    Debug.LogError($"node {nodeKey} not exists");
                    break;
                }
            }
            while (nextNodeFound);

            route = new(routeTargets.ColorKey, points);
            return true;
        }

        public bool TryRebuildRoute(IDraggableRoutePoint newPoint, IRoutePoint oldPoint )
        {
            return false;
        }
    
    }
}
