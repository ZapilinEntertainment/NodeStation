using UnityEngine;

namespace ZE.NodeStation
{
    public interface IRoutePoint : IColliderOwner
    {
        bool TryReceive(IDraggableRoutePoint point);
    
    }
}
