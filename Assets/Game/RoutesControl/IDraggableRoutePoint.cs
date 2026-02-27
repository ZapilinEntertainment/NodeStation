using UnityEngine;

namespace ZE.NodeStation
{
    public interface IDraggableRoutePoint : IColliderOwner
    {
        IRoute Route { get; }
        IPathNode Node { get; }
        int RouteIndex { get; }
    
    }
}
