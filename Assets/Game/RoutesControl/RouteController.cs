using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace ZE.NodeStation
{
    // controls route changes
    public class RouteController : IDisposable
    {
        public readonly int TimetabledTargetNodeKey;
        public IPathNode CurrentExitNode { get; private set; }
        public IRoute Route => _route;

        private readonly TrainRoute _route;
        private readonly IMessageBroker _messageBroker;
        

        public RouteController(IMessageBroker messageBroker, TrainRoute route, int timetabledTargetNodeKey)
        {
            _messageBroker = messageBroker;
            _route = route;
            TimetabledTargetNodeKey = timetabledTargetNodeKey;
        }

        public void Dispose()
        {
            _route.Dispose();
        }

        public void UpdatePoints(List<IPathNode> points)
        {
            _route.UpdatePoints(points);
            CurrentExitNode = points[points.Count - 1];
            UpdateRouteStatus();
            _messageBroker.Publish<RouteChangedMessage>(new(this));
        }

        public void SetCurrentExitNode(IPathNode targetNode)
        {
            CurrentExitNode = targetNode;
            UpdateRouteStatus();
        }

        private void UpdateRouteStatus()
        {
            _route.Status = _route.Points[_route.Points.Count - 1].Key == TimetabledTargetNodeKey ? RouteStatus.Correct : RouteStatus.Missed;
        }
    }
}
