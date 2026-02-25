using System;
using UnityEngine;

namespace ZE.NodeStation
{
    [Serializable]
    public struct MapPosition
    {
        public PathKey Path;
        public float Percent;
        [Header("IsReversed sets only point direction, Percent will be counted from path start anyway")]
        public bool IsReversed;    
    }
}
