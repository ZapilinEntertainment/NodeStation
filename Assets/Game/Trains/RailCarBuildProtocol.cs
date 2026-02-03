using System;
using UnityEngine;

namespace ZE.NodeStation
{
    [Serializable]
    public struct RailCarBuildProtocol
    {
        public bool ReverseCarViewDirection;
        public RailCarConfiguration Configuration;
        // there would be colour settings
    }
}
