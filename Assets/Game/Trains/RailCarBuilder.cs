using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class RailCarBuilder
    {
        private BogieView _bogieViewPrefab;


        public RailCar Build(RailCarBuildProtocol protocol)
        {
            var config = protocol.Configuration;
            var railCar = new RailCar(
                new(config.FrontBogeyOffset), 
                new(config.RearBogeyOffset), 
                config.CarLength, 
                protocol.ReverseCarViewDirection);

            var view = GameObject.Instantiate<RailCarView>(config.Prefab);
            view.AssignOwner(railCar);

            _bogieViewPrefab ??= Resources.Load<BogieView>(ResourceNames.BOGIE_PREFAB_NAME);
            AddBogie(railCar.FrontBogie, view.transform, true);
            AddBogie(railCar.RearBogie, view.transform, false);

            return railCar;
        }

        private void AddBogie(Bogie bogie, Transform parent, bool isFront)
        {
            var view = GameObject.Instantiate(_bogieViewPrefab, parent);
            view.AssignOwner(bogie);

            #if UNITY_EDITOR
                view.name = isFront ? DebugConstants.FRONT_BOGIE_NAME : DebugConstants.REAR_BOGIE_NAME;
            #endif
        }    
    }
}
