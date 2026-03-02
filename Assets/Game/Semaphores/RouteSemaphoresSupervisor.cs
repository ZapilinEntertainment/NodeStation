using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UniRx;
using System.Linq;

namespace ZE.NodeStation
{
    // creates semaphores contoller for each new route
    public class RouteSemaphoresSupervisor : IDisposable, IFixedFrameTickable
    {
        private struct TrackingRoute
        {
            public TimetabledTrain TimetabledTrain;
            public IRoute Route;
        }

        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly RouteSemaphoreControllerBuilder _builder;
        private readonly Dictionary<TrackingRoute, RouteSemaphoresController> _controllers = new();
        private readonly List<TrackingRoute> _clearList = new();
        private readonly RoutesManager _routesManager;

        [Inject]
        public RouteSemaphoresSupervisor(
            IMessageBroker messageBroker, 
            TickableManager tickableManager,
            RouteSemaphoreControllerBuilder controllersBuilder,
            RoutesManager routesManager)
        {
            _builder = controllersBuilder;
            _routesManager = routesManager;

            messageBroker.Receive<TrainAnnouncedMessage>()
                .Subscribe(OnTrainAnnounced)
                .AddTo(_compositeDisposable);

            messageBroker.Receive<RouteChangedMessage>()
                .Subscribe(OnRouteChanged)
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
                var timetabledTrain = controllerKvp.Key.TimetabledTrain;
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
                                _clearList.Add(controllerKvp.Key);
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
            if (!_routesManager.TryGetRoute(train, out var route))
                return;

            var controller = _builder.Build(train, route);
            if (controller != null)
                _controllers.Add(new() { TimetabledTrain = train, Route = route }, controller);
            else
                Debug.LogError("route semaphore controller build error");
        }

        private void OnRouteChanged(RouteChangedMessage msg)
        {
            var keys = _controllers.Keys.ToArray();
            foreach (var key in keys)
            {                
                if (key.Route == msg.Route)
                {
                    var previousController = _controllers[key];                  
                    var controller = _builder.Rebuild(previousController, key.Route);
                    if (controller != null)
                    {
                        _controllers[key] = controller;
                    }                        
                    else
                    {
                        _clearList.Add(key);
                        Debug.LogError("route semaphore controller rebuild error");
                    }
                    previousController.Dispose();
                }
                // multiple train can be on same route (in theory) - no break
            }
        }
    }
}
