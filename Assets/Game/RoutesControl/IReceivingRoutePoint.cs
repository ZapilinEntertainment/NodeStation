using UnityEngine;

namespace ZE.NodeStation
{
    public interface IReceivingRoutePoint : IColliderOwner
    {
        IPathNode Node { get; }
    
    }
}
