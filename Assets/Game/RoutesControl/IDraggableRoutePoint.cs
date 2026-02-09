using UnityEngine;

namespace ZE.NodeStation
{
    public interface IDraggableRoutePoint : IColliderOwner
    {
        TrainRoute Route { get; }
        IPathNode Node { get; }
        int RouteIndex { get; }
    
    }
}
