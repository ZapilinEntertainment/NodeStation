using UnityEngine;

namespace ZE.NodeStation
{
    public readonly struct RailMovement
    {
        public readonly float Distance;
        public readonly bool IsReversed; // false by default    
    
        public RailMovement(float distance, bool isReversed)
        {
            Distance = distance;
            IsReversed = isReversed;
        }
    }
}
