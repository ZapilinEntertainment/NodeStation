using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    public class RouteDrawer : IDisposable
    {
        private readonly ILineDrawer _lineDrawer;
        private readonly List<IPointDrawer> _activePointMarkers = new();
        private readonly List<Vector3> _points = new();

        public RouteDrawer(ILineDrawer lineDrawer) 
        {
            _lineDrawer = lineDrawer;
        }

        public void AddNodeDrawer(IPointDrawer drawer) => _activePointMarkers.Add(drawer);

        public void AddLinePoint(Vector3 end) => _points.Add(end);

        public void Dispose()
        {
            _points.Clear();
            _lineDrawer?.Dispose();

            if (_activePointMarkers.Count != 0)
            {
                foreach (var point in _activePointMarkers)
                {
                    point.Dispose();
                }
                _activePointMarkers.Clear();
            }            
        }

        public void FinishDraw()
        {
            var pointsArray = _points.ToArray();
            var up = Vector3.up;
            for (var i = 0; i< pointsArray.Length; i++)
            {
                pointsArray[i] = pointsArray[i] + up * 0.01f;
            }
            _lineDrawer.DrawPoints(pointsArray);
        }
    }
}
