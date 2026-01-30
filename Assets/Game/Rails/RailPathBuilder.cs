using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    public class RailPathBuilder
    {
        private readonly IReadOnlyDictionary<int, NodePoint> _points;

        public RailPathBuilder(IReadOnlyDictionary<int, NodePoint> points) 
        { 
            _points = points; 
        }

        public IRailPath Build(in PathKey key, ConstructingPathData data)
        {
            var startPos = _points[data.StartNodeKey].transform.position;
            var endPos = _points[data.EndNodeKey].transform.position;
            return new LinearRail(key, startPos, endPos);
        }
    
    }
}
