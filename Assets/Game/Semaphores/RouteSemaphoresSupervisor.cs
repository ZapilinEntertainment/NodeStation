using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UniRx;

namespace ZE.NodeStation
{
    // creates semaphores contoller for each new route
    public class RouteSemaphoresSupervisor : IDisposable, IFixedFrameTickable
    {
        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly RouteSemaphoreControllerBuilder _builder;
        private readonly Dictionary<TimetabledTrain, RouteSemaphoresController> _controllers = new();
        private readonly List<TimetabledTrain> _clearList = new();

        [Inject]
        public RouteSemaphoresSupervisor(
            IMessageBroker messageBroker, 
            TickableManager tickableManager,
            RouteSemaphoreControllerBuilder controllersBuilder)
        {
            _builder = controllersBuilder;

            messageBroker.Receive<TrainAnnouncedMessage>()
                .Subscribe(OnTrainAnnounced)
                .AddTo(_compositeDisposable);

            tickableManager.AddAsSubscription(this).AddTo(_compositeDisposable);
        }

        public void Tick()
        {
            // NOTE: shouldn't subscribe to route / train updates,
            // because lights controller can live longer than route / timetabled train

            var dt = Time.fixedDeltaTime;
            foreach (var controllerKvp in _controllers) 
            { 
                var timetabledTrain = controllerKvp.Key;
                var semaphoreController = controllerKvp.Value;

                switch(timetabledTrain.Status) 
                {
                    case TimetabledTrainStatus.Announced:
                        {
                            semaphoreController.OnTrainMove(timetabledTrain.MaxSpeed * dt);
                            break;
                        }
                    case TimetabledTrainStatus.Launched:
                        {
                            semaphoreController.OnTrainMove(timetabledTrain.Train.Speed * dt); 
                            break;
                        }
                    default:
                        {
                            semaphoreController.OnTrainMove(timetabledTrain.MaxSpeed * dt);
                            if (semaphoreController.ActiveSemaphoresCount == 0)
                            {
                                semaphoreController.Dispose();
                                _clearList.Add(timetabledTrain);
                            }
                            break;
                        }
                }
            }

            if (_clearList.Count != 0)
            {
                foreach (var train in _clearList)
                {
                    _controllers.Remove(train);
                }
                _clearList.Clear();
            }
        }

        public void Dispose() 
        {
            _clearList.Clear();
            foreach (var controller in _controllers.Values)
            {
                controller.Dispose();
            }
            _controllers.Clear();
            _compositeDisposable.Dispose();
        }        

        private void OnTrainAnnounced(TrainAnnouncedMessage msg)
        {
            var train = msg.Train;
            var controller = _builder.Build(train);
            if (controller != null)
                _controllers.Add(train, controller);
            else
                Debug.LogError("route semaphore controller build error");
        }
    }
}
