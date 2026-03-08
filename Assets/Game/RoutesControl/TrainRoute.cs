using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace ZE.NodeStation
{
    public class TrainRoute : IRoute
    {
        public ColorKey ColorKey { get;private set; }
        public IReadOnlyList<IPathNode> Points => _points;
        public IReadOnlyReactiveProperty<RouteStatus> StatusProperty => _statusProperty;

        // not available via interface
        public RouteStatus Status
        {
            get => _statusProperty.Value;
            set => _statusProperty.Value = value;
        }

        private List<IPathNode> _points;
        private readonly ReactiveProperty<RouteStatus> _statusProperty = new();


        public TrainRoute(ColorKey colorKey, List<IPathNode> points)
        {
            ColorKey = colorKey;
            _points = new(points);
        }
    
        public void Dispose()
        {
            _points.Clear();
            _statusProperty.Dispose();
        }

        public IEnumerator<IPathNode> GetEnumerator() => _points.GetEnumerator();

        public void UpdatePoints(List<IPathNode> points) 
        {
            _points = points;
        }

        public bool TryGetNextPoint(IPathNode point, out IPathNode nextPoint) 
        { 
            var count = _points.Count;
            for (var i = 0; i < count; i++)
            {
                if (_points[i] == point && count - i != 1)
                {
                    nextPoint = _points[i + 1];
                    return true;
                }                    
            }
            nextPoint = null;
            return false;
        }
    }
}
