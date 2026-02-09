using UnityEngine;

namespace ZE.NodeStation
{
    public static class LayerMasks
    {
        public const int USER_DRAGGABLE_LAYER = 6;
        public const int ROUTE_POINTS_LAYER = 7;

        public const int USER_DRAGGABLE_MASK = (1 << USER_DRAGGABLE_LAYER);
        public const int DRAGGABLES_RECEIVERS_MASK = (1 << ROUTE_POINTS_LAYER);
    
    }
}
