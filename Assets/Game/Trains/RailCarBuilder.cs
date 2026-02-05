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
            RailCar railCar;
            if (!protocol.ReverseCarViewDirection)
            {
                railCar = new RailCar(
                new(config.FrontBogeyOffset),
                new(config.RearBogeyOffset),
                config.CarLength,
                false);
            }
            else
            {
                railCar = new RailCar(
                new(-config.RearBogeyOffset),
                new(-config.FrontBogeyOffset),
                config.CarLength,
                true);
            }            

            var view = GameObject.Instantiate<RailCarView>(config.Prefab);
            view.AssignOwner(railCar);

            _bogieViewPrefab ??= Resources.Load<BogieView>(ResourceNames.BOGIE_PREFAB_NAME);
            AddBogie(railCar.FrontBogie, true);
            AddBogie(railCar.RearBogie, false);

            return railCar;
        }

        private void AddBogie(Bogie bogie, bool isFront)
        {
            var view = GameObject.Instantiate(_bogieViewPrefab);
            view.AssignOwner(bogie);

            #if UNITY_EDITOR
                view.name = isFront ? DebugConstants.FRONT_BOGIE_NAME : DebugConstants.REAR_BOGIE_NAME;
            #endif
        }    
    }
}
