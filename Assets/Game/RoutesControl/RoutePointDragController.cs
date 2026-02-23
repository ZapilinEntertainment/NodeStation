using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public class RoutePointDragController : RoutePointController, IDraggableRoutePoint
    {        
        private readonly TrainRoute _route;        
        private readonly RoutePointDrawer _drawer;        
        private readonly int _routeIndex;

        public RoutePointDragController(
            CollidersManager collidersManager, 
            TrainRoute route, 
            IPathNode node, 
            RoutePointDrawer drawer,
            int routeIndex) : base(collidersManager, node)
        {            
            _route = route;            
            _drawer = drawer;
            _routeIndex = routeIndex;            
        }

        public TrainRoute Route => _route;
        public int RouteIndex => _routeIndex;

        public override int GetColliderId() => _drawer.DragCollider.GetInstanceID();        
    }
}
