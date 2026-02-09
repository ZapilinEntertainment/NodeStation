using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    public enum RouteStatus : byte { Undefined, Complete, Incomplete, Blocked, RouteIntersections}

    public struct RouteCheckResult
    {
        public RouteStatus Status;
        public int LastNodeKey;

        public static RouteCheckResult Undefined => new() { Status = RouteStatus.Undefined};
        public static RouteCheckResult Completed => new() { Status = RouteStatus.Complete };
    }

    public class RouteApplyController
    {
        private readonly PathsMap _map;
        private readonly Dictionary<int, TrainRoute> _routeAffiliations = new();
        
        public RouteApplyController(PathsMap map)
        {
            _map = map;
        }

        public RouteCheckResult ApplyRoute(TrainRoute route)
        {
            var points = route.Points;
            var routeLength = points.Count;

            for (var i = 0; i < routeLength; i++)
            {
                var nodeKey = points[i];
                var isFirstNode = i == 0;

                if (!_map.TryGetNode(nodeKey, out var node))
                {
                    if (isFirstNode)
                        return RouteCheckResult.Undefined;
                    else
                        return new() { Status = RouteStatus.Incomplete, LastNodeKey = points[i-1] };
                }
                else
                {
                    if (isFirstNode)
                    {
                        if (!node.TrySetupPath(Constants.NO_EXIT_PATH_CODE, points[i + 1]))
                            return RouteCheckResult.Undefined;
                    }
                    else
                    {
                        var nextNodeKey = i == routeLength - 1 ? Constants.NO_EXIT_PATH_CODE : points[i + 1];
                        var prevNodeKey = points[i-1];
                        if (!node.TrySetupPath(prevNodeKey, nextNodeKey))
                            return new() { Status = RouteStatus.Blocked, LastNodeKey = prevNodeKey };
                    }

                    if (_routeAffiliations.TryGetValue(nodeKey, out var overlappingRoute) 
                        && overlappingRoute != route)
                        return new() { Status = RouteStatus.RouteIntersections, LastNodeKey = nodeKey};

                    _routeAffiliations[nodeKey] = route;
                }
            }

            return RouteCheckResult.Completed;
        }

        public void ClearRouteAffiliations(TrainRoute route)
        {
            foreach (var node in route.Points)
            {
                if (_routeAffiliations.TryGetValue(node, out var affiliatedRoute) && affiliatedRoute == route)
                    _routeAffiliations.Remove(node);
            }
        }
    }
}
