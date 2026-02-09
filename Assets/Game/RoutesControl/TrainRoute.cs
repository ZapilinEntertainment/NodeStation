using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    public class TrainRoute : IDisposable
    {
        public readonly ColorKey ColorKey;
        public IReadOnlyList<IPathNode> Points => _points;
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

        public void UpdatePoints(List<IPathNode> points) => _points = points;
    }
}
