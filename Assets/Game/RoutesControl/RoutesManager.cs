using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UniRx;

namespace ZE.NodeStation
{
    public class RoutesManager : IDisposable
    {
        private readonly IMessageBroker _messageBroker;
        private readonly Dictionary<TimetabledTrain, RouteController> _routes = new();
        private readonly Dictionary<IRoute, RouteController> _routeControllers = new();

        [Inject]
        public RoutesManager(IMessageBroker messageBroker)
        {
            _messageBroker = messageBroker;
        }

        public void SetRoute(TimetabledTrain train, RouteController routeController)
        {
            _routes.Add(train, routeController);
            _routeControllers.Add(routeController.Route, routeController);
            train.DisposeEvent += () => this.ClearRoute(train);
        }

        public void ClearRoute(TimetabledTrain train)
        {
            if (_routes.TryGetValue(train, out var route))
            {
                _routeControllers.Remove(route.Route);
                route.Dispose();
                _routes.Remove(train);                
                _messageBroker.Publish<RouteDisposedMessage>(new(route));
            }
        }

        public bool TryGetRoute(TimetabledTrain train, out IRoute route)
        {
            if (_routes.TryGetValue(train, out var routeController))
            {
                route = routeController.Route;
                return true;
            }

            route = default;
            return false;
        }

        public bool TryGetRouteController(IRoute route, out RouteController routeController) =>
            _routeControllers.TryGetValue(route, out routeController);

        public void Dispose()
        {
            foreach (var route in _routes.Values)
            {
                route?.Dispose();
            }
            _routes.Clear();
            _routeControllers.Clear();
        }
    }
}
