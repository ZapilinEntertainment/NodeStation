using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public class RoutePointReceiveController : RoutePointController, IReceivingRoutePoint
    {
        private readonly RoutePointDrawer _drawer;

        public RoutePointReceiveController(CollidersManager collidersManager, IPathNode node, RoutePointDrawer pointDrawer) :
            base(collidersManager, node) 
        {
            _drawer = pointDrawer;
        }
        

        public override int GetColliderId() => _drawer.ReceiveCollider.GetInstanceID();
    }
}
