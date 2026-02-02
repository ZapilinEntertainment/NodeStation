using System;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class TrainFactory : ILifetimeObject, IDisposable
    {
        public event Action DisposedEvent;
        private readonly TrainViewFactory _trainViewFactory;
        private readonly TrainBase.InjectProtocol _injectProtocol;

        [Inject]
        public TrainFactory(
            RailMovementCalculator railMovementCalculator, 
            PathsMap pathsMap, 
            TickableManager tickableManager,
            TrainViewFactory viewFactory) 
        {
            _injectProtocol = new(railMovementCalculator, pathsMap, tickableManager);
            _trainViewFactory = viewFactory;
        }        

        public ITrain Build(TrainConfiguration config, RailPosition position)
        {
            var train = new TrainBase(_injectProtocol, config, lifetimeObject: this);
            train.SetPosition(position);

            var view = _trainViewFactory.Build();
            view.AssignOwner(train);

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
