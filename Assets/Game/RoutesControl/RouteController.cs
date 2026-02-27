using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace ZE.NodeStation
{
    // controls route changes
    public class RouteController : IDisposable
    {
        public IRoute Route => _route;
        private readonly TrainRoute _route;
        private readonly IMessageBroker _messageBroker;

        public RouteController(IMessageBroker messageBroker, TrainRoute route)
        {
            _messageBroker = messageBroker;
            _route = route;
        }

        public void Dispose()
        {
            _route.Dispose();
        }

        public void UpdatePoints(List<IPathNode> points)
        {
            _route.UpdatePoints(points);
            _messageBroker.Publish<RouteChangedMessage>(new(this));
        }
    }
}
