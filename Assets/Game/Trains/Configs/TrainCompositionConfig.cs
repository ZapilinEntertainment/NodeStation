using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation 
{ 
[CreateAssetMenu(fileName = nameof(TrainCompositionConfig), menuName = Constants.ScriptableObjectsFolderPath + nameof(TrainCompositionConfig))]
    public class TrainCompositionConfig : TrainCompositionBase
    {
        [SerializeField] private RailCarBuildProtocol[] _railCars;

        public override IReadOnlyList<RailCarBuildProtocol> RailCarProtocols => _railCars;

        public override float CalculateTrainLength()
        {
            var length = 0f;
            foreach (var railCar in _railCars)
            {
                length += railCar.Configuration.CarLength;
            }
            return length;
        }
    }
}
