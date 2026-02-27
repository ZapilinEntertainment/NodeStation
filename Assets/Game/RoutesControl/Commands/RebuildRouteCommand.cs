using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class RebuildRouteCommand
    {
        private readonly RoutesManager _routesManager;
        private readonly PathsMap _map;

        [Inject]
        public RebuildRouteCommand(RoutesManager routesManager, PathsMap map)
        {
            _routesManager = routesManager;
            _map = map;
        }

        public bool TryExecute(IDraggableRoutePoint draggingPoint, IPathNode receivingPoint)
        {
            var route = draggingPoint.Route;
            if (!_routesManager.TryGetRouteController(route, out var controller))
                return false;

            var changingNode = draggingPoint.Node;
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
            controller.UpdatePoints(newPoints);
            return true;
        }
    
    }
}
