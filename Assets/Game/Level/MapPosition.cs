using System;
using UnityEngine;

namespace ZE.NodeStation
{
    [Serializable]
    public struct MapPosition
    {
        public PathKey Path;
        public float Percent;
        public bool IsReversed;    
    }
}
