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

        // just get next point until path ends
        public bool TryBuildRoute(int startNode, ColorKey colorKey, out TrainRoute route)
        {
            var points = new List<IPathNode>();
            var nodeKey = startNode;
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
                {
                    //Debug.Log($"stop route: ${prevNodeKey} -> {nextNodeKey}");
                    break;
                }
                    

                prevNodeKey = nodeKey;
                nodeKey = nextNodeKey;       

                if (!_map.TryGetNode(nodeKey, out node))
                {
                    Debug.LogError($"node {nodeKey} not exists");
                    break;
                }
            }
            while (nextNodeFound);

            route = new(colorKey, points);
            return true;
        }

        public bool TryRebuildRoute(IDraggableRoutePoint draggingPoint, IPathNode receivingPoint)
        {
            var changingNode = draggingPoint.Node;
            var route = draggingPoint.Route;
            var routePoints = route.Points;
            var newPoints = new List<IPathNode>();

            var prevRouteNodeKey = Constants.NO_EXIT_PATH_CODE;
            var exitNodeKey = Constants.NO_EXIT_PATH_CODE;

            for (var i = 0; i < routePoints.Count; i++)
            {
                if (routePoints[i] == changingNode)
                {
                    if (i != 0)
                        prevRouteNodeKey = routePoints[i - 1].Key;
                    else
                        Debug.LogWarning("Attention: dividing at start route point");

                    if (receivingPoint.TryGetExitNode(prevRouteNodeKey, out exitNodeKey)
                        && receivingPoint.TrySetupPath(prevRouteNodeKey, exitNodeKey))
                    {
                        // switch success
                        newPoints.Add(receivingPoint);
                        break;
                    }
                    else
                    {
                        // TODO: some notification
                        Debug.LogWarning($"cannot reach this point. {prevRouteNodeKey} -> {receivingPoint.Key} -> {exitNodeKey}");                        
                        return false;
                    }
                }
                else
                {
                    newPoints.Add(routePoints[i]);
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
                    newPoints.Add(exitNode);
                    prevRouteNodeKey = node.Key;
                    node = exitNode;

                    nextNodeFound = node.TryGetExitNode(prevRouteNodeKey, out exitNodeKey);
                    if (!nextNodeFound)
                    {
                       // Debug.Log($"stop route: ${prevRouteNodeKey} -> {exitNodeKey}");
                        break;
                    }
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
