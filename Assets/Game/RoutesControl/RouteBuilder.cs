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
            var points = new List<IPathNode>();
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
                points.Add(node);

                nextNodeFound = node.TryGetExitNode(prevNodeKey, out var nextNodeKey);
                if (!nextNodeFound)
                    break;

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

        public bool TryRebuildRoute(IDraggableRoutePoint draggingPoint, IPathNode receivingPoint)
        {
            var changingNode = draggingPoint.Node;
            var route = draggingPoint.Route;
            var points = route.Points;
            var newPoints = new List<IPathNode>();

            var prevRouteNodeKey = Constants.NO_EXIT_PATH_CODE;
            var exitNodeKey = Constants.NO_EXIT_PATH_CODE;

            for (var i = 0; i < points.Count; i++)
            {
                if (points[i] == changingNode)
                {
                    if (i != 0)
                        prevRouteNodeKey = points[i - 1].Key;

                    if (receivingPoint.TryGetExitNode(prevRouteNodeKey, out exitNodeKey)
                        && receivingPoint.TrySetupPath(prevRouteNodeKey, exitNodeKey))
                    {
                        // switch success
                        break;
                    }
                    else
                    {
                        // TODO: some notification
                        Debug.Log("cannot reach this point");
                        return false;
                    }
                }
                else
                {
                    newPoints.Add(points[i]);
                }
            }

            // route part before new switch point has checked, continue new route:
            // (exit node key and previous node are already calculated for switching point)
            var node = receivingPoint;
            var nextNodeFound = true;
            do
            {
                if (_map.TryGetNode(exitNodeKey, out var exitNode))
                {
                    newPoints.Add(node);
                    prevRouteNodeKey = node.Key;
                    node = exitNode;

                    nextNodeFound = node.TryGetExitNode(prevRouteNodeKey, out exitNodeKey);
                }
                else
                {
                    Debug.LogError($"route changing failed: node {exitNode} not exists");
                    return false;
                }
            }
            while (nextNodeFound);

            // completed:
            route.UpdatePoints(newPoints);
            return true;
        }
    
    }
}
