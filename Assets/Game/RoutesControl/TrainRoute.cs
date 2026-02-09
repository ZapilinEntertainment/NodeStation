using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    public class TrainRoute : IDisposable
    {
        public readonly ColorKey ColorKey;
        public IReadOnlyList<int> Points => _points;
        private readonly List<int> _points;

        public TrainRoute(ColorKey colorKey, List<int> points)
        {
            ColorKey = colorKey;
            _points = new(points);
        }
    
        public void Dispose()
        {
            _points.Clear();
        }

        public IEnumerator<int> GetEnumerator() => _points.GetEnumerator();
    }
}
