using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    public abstract class TrainCompositionBase : ScriptableObject
    {
        public abstract IReadOnlyList<RailCarBuildProtocol> RailCarProtocols { get; }

        public abstract float CalculateTrainLength();

        // distance between first bogie and locomotive front edge
        public abstract float GetFrontOverhang();

        // distance between last car rear bogie and car's rear edge
        public abstract float GetRearOverhang();

        // |<------------TrainLength----------------->|
        // |RearOverhang - SpawnOffset - FrontOverhang|
        public float GetFirstBogieSpawnOffset() => CalculateTrainLength() - GetRearOverhang() - GetFrontOverhang();
    }
}
