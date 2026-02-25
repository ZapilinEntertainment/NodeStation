using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    public class TrainRoute : IDisposable
    {
        public readonly ColorKey ColorKey;
        public IReadOnlyList<IPathNode> Points => _points;
        public Action PointsChangedEvent;
        private List<IPathNode> _points;

        public TrainRoute(ColorKey colorKey, List<IPathNode> points)
        {
            ColorKey = colorKey;
            _points = new(points);
        }
    
        public void Dispose()
        {
            _points.Clear();
        }

        public IEnumerator<IPathNode> GetEnumerator() => _points.GetEnumerator();

        public void UpdatePoints(List<IPathNode> points) 
        {
            _points = points;
            PointsChangedEvent?.Invoke();
        }

        public bool TryGetNextPoint(IPathNode prevPoint, out IPathNode nextPoint) 
        { 
            var count = _points.Count;
            for (var i = 0; i < count; i++)
            {
                if (_points[i] == prevPoint && count - i != 1)
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
