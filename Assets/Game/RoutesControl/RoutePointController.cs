using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public class RoutePointController : IDraggableRoutePoint, IDisposable
    {
        public IPathNode Node => _node;

        private readonly CollidersManager _collidersManager;
        private readonly TrainRoute _route;
        private readonly IPathNode _node;
        private readonly RoutePointDrawer _drawer;
        private readonly int _colliderKey;
        private readonly int _routeIndex;

        public RoutePointController(
            CollidersManager collidersManager, 
            TrainRoute route, 
            IPathNode node, 
            RoutePointDrawer drawer,
            int routeIndex)
        {
            _collidersManager = collidersManager;
            _route = route;
            _node = node;
            _drawer = drawer;
            _routeIndex = routeIndex;

            _colliderKey = _collidersManager.Register(this);
        }

        public TrainRoute Route => _route;
        public int RouteIndex => _routeIndex;

        public int GetColliderId() => _drawer.DragCollider.GetInstanceID();

        public void Dispose() => _collidersManager?.Unregister(_colliderKey);
    }
}
