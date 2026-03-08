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
        private IPathNode _targetNode;

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
            UpdateRouteStatus();
            _messageBroker.Publish<RouteChangedMessage>(new(this));
        }

        public void SetTargetNode(IPathNode targetNode)
        {
            _targetNode = targetNode;
            UpdateRouteStatus();
        }

        private void UpdateRouteStatus()
        {
            if (_targetNode == null)
            {
                _route.Status = RouteStatus.Correct;
                return;
            }

            _route.Status = _route.Points[_route.Points.Count - 1] == _targetNode ? RouteStatus.Correct : RouteStatus.Missed;
        }
    }
}
