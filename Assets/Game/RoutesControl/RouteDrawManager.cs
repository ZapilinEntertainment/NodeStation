using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class RouteDrawManager : IDisposable
    {
        private readonly PathsMap _map;
        private readonly Dictionary<TrainRoute, RouteDrawer> _drawers = new();
        private readonly RouteDrawerFactory _routeDrawerFactory;

        [Inject]
        public RouteDrawManager(PathsMap map, RouteDrawerFactory routeDrawerFactory)
        {
            _map = map;
            _routeDrawerFactory = routeDrawerFactory;
        }        

        public void DrawRoute(TrainRoute route)
        {
            var drawer = _routeDrawerFactory.Create(route);
            var points = route.Points;
            for (var i = 0; i < points.Count; i++)
            {
                var isNotFirstPoint = i != 0;

                if (!_map.TryGetNode(points[i], out var node))
                {
                    Debug.LogError($"node {points[i]} not exists!");
                    i++;
                    continue;
                }
                else
                {
                   var canBeControlledByPlayer =
                    isNotFirstPoint
                    && (_map.TryGetNode(points[i - 1], out var prevNode))
                    && prevNode.HaveMultipleExits;

                    drawer.AddNode(node.WorldPosition, canBeControlledByPlayer);
                }

                // TODO: add other segment draw options
                drawer.AddLinePoint(node.WorldPosition);
            }
            drawer.Redraw();

            _drawers.Add(route, drawer);
        }

        public void ClearRouteDrawing(TrainRoute route)
        {
            if (_drawers.TryGetValue(route, out var drawer))
            {
                drawer?.Dispose();
                _drawers.Remove(route);
            }
        }

        public void Dispose()
        {
            foreach (var drawer in _drawers.Values)
                drawer.Dispose();

            _drawers.Clear();
        }
    }
}
