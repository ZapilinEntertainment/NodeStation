using System;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class TrainFactory : ILifetimeObject, IDisposable
    {
        public event Action DisposedEvent;
        private readonly TrainBase.InjectProtocol _injectProtocol;
        private readonly RailCarBuilder _railCarBuilder;

        [Inject]
        public TrainFactory(
            RailMovementCalculator railMovementCalculator, 
            PathsMap pathsMap, 
            TickableManager tickableManager,
            RailCarBuilder railCarBuilder) 
        {
            _injectProtocol = new(railMovementCalculator, pathsMap, tickableManager);
            _railCarBuilder = railCarBuilder;
        }  
        
        public ITrain Build(TrainConfiguration config, RailPosition position, params RailCarBuildProtocol[] protocols)
        {
            var train = new MultiBogeysTrain(_injectProtocol, config, lifetimeObject: this);

            var carsCount = protocols.Length;
            var cars = new RailCar[carsCount];
            for (var i = 0; i< carsCount; i++)
            {
                cars[i] = _railCarBuilder.Build(protocols[i]);
            }

            train.SetupTrain(cars);
            train.SetPosition(position);
            train.Activate();

            return train;
        }

        public void Dispose()
        {
            if (DisposedEvent != null)
            {
                DisposedEvent.Invoke();
                DisposedEvent = null;
            }
        }
    }
}
