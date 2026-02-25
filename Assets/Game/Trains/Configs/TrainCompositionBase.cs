using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    public abstract class TrainCompositionBase : ScriptableObject
    {
        public abstract IReadOnlyList<RailCarBuildProtocol> RailCarProtocols { get; }

        public abstract float CalculateTrainLength();
    }
}
