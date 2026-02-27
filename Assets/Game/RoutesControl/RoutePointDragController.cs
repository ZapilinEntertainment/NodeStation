using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public class RoutePointDragController : RoutePointController, IDraggableRoutePoint
    {        
        private readonly IRoute _route;        
        private readonly RoutePointDrawer _drawer;        
        private readonly int _routeIndex;

        public RoutePointDragController(
            CollidersManager collidersManager,
            IRoute route, 
            IPathNode node, 
            RoutePointDrawer drawer,
            int routeIndex) : base(collidersManager, node)
        {            
            _route = route;            
            _drawer = drawer;
            _routeIndex = routeIndex;            
        }

        public IRoute Route => _route;
        public int RouteIndex => _routeIndex;

        public override int GetColliderId() => _drawer.DragCollider.GetInstanceID();        
    }
}
