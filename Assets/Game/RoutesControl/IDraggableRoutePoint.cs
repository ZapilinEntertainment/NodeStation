using UnityEngine;

namespace ZE.NodeStation
{
    public interface IDraggableRoutePoint : IRoutePoint
    {
        public TrainRoute Route { get; }
    
    }
}
