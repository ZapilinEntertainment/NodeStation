using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    public enum RouteStatus : byte { Undefined, Complete, Incomplete, Blocked, RouteIntersections}

    public struct RouteCheckResult
    {
        public RouteStatus Status;
        public IPathNode LastNode;

        public static RouteCheckResult Undefined => new() { Status = RouteStatus.Undefined};
        public static RouteCheckResult Completed => new() { Status = RouteStatus.Complete };
    }

    public class RouteApplyController
    {
        private readonly PathsMap _map;
        private readonly Dictionary<IPathNode, TrainRoute> _routeAffiliations = new();
        
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
                var node = points[i];
                var isFirstNode = i == 0;

                if (isFirstNode)
                {
                    if (!node.TrySetupPath(Constants.NO_EXIT_PATH_CODE, points[i + 1].Key))
                        return RouteCheckResult.Undefined;
                }
                else
                {
                    var nextNodeKey = i == routeLength - 1 ? Constants.NO_EXIT_PATH_CODE : points[i + 1].Key;
                    var prevNode = points[i - 1];
                    if (!node.TrySetupPath(prevNode.Key, nextNodeKey))
                        return new() { Status = RouteStatus.Blocked, LastNode = prevNode };
                }

                if (_routeAffiliations.TryGetValue(node, out var overlappingRoute)
                    && overlappingRoute != route)
                    return new() { Status = RouteStatus.RouteIntersections, LastNode = node };

                _routeAffiliations[node] = route;
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
