using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class RouteDrawManager : IDisposable
    {
        private readonly Dictionary<TrainRoute, RouteDrawer> _drawers = new();
        private readonly RouteDrawerFactory _routeDrawerFactory;

        [Inject]
        public RouteDrawManager(RouteDrawerFactory routeDrawerFactory)
        {
            _routeDrawerFactory = routeDrawerFactory;
        }        

        public void DrawRoute(TrainRoute route, bool redraw = false)
        {
            if (_drawers.ContainsKey(route))
            {
                if (redraw)
                    ClearRouteDrawing(route);
                else
                    return;
            }
            var drawer = _routeDrawerFactory.Create(route);
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
