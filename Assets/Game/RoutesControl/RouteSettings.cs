using System;
using UnityEngine;

namespace ZE.NodeStation
{
    [Serializable]
    public struct RouteSettings
    {
        public int SpawnNodeKey;
        public bool IsReversed;

        public int TargetNodeKey;    
        public ColorKey ColorKey;
    }
}
