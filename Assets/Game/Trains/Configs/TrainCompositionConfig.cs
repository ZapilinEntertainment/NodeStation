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

        public override float GetFrontOverhang()
        {
            if (_railCars.Length == 0)
                return 0f;

            var config = _railCars[0].Configuration;
            if (config == null)
                return 0f;

           
            return config.CarLength * 0.5f - config.FrontBogeyOffset;
        }

        public override float GetRearOverhang()
        {
            if (_railCars.Length == 0)
                return 0f;

            var config = _railCars[_railCars.Length - 1].Configuration;
            if (config == null)
                return 0f;


            return config.CarLength * 0.5f - Mathf.Abs(config.RearBogeyOffset);
        }
    }
}
