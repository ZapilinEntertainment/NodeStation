using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class TrainFactory : ILifetimeObject, IDisposable
    {
        public event Action DisposedEvent;
        private readonly TrainBase.InjectProtocol _injectProtocol;
        private readonly RailCarBuilder _railCarBuilder;
        private readonly SceneViewsList _sceneViewsList;

        [Inject]
        public TrainFactory(
            RailMovementCalculator railMovementCalculator, 
            PathsMap pathsMap, 
            TickableManager tickableManager,
            RailCarBuilder railCarBuilder,
            SceneViewsList sceneViewsList) 
        {
            _injectProtocol = new(railMovementCalculator, pathsMap, tickableManager);
            _railCarBuilder = railCarBuilder;
            _sceneViewsList = sceneViewsList;
        }  
        
        public ITrain Build(TrainConfiguration config, RailPosition position)
        {
            var train = new MultiBogeysTrain(_injectProtocol, config, lifetimeObject: this);

            var protocols = config.TrainCompositionConfig.RailCarProtocols;
            var carsCount = protocols.Count;
            var cars = new RailCar[carsCount];
            for (var i = 0; i< carsCount; i++)
            {
                cars[i] = _railCarBuilder.Build(protocols[i]);
            }

            train.SetupTrain(cars);
            train.SetPosition(position);
            train.Activate();

            var view = new GameObject("train view").AddComponent<MultiBogeyTrainViewController>();            
            view.Init(train, _sceneViewsList);
            _sceneViewsList.RegisterView(view);

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
