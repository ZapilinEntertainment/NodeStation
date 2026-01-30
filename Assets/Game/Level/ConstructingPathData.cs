using System;
using UnityEngine;

namespace ZE.NodeStation
{
    [Serializable]
    public struct ConstructingPathData
    {
        public int StartNodeKey;
        public int EndNodeKey;
        public PathType Type;    
    }
}
