using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    public class RouteDrawer : IDisposable
    {
        private readonly ColorKey _color;
        private readonly PointDrawerFactory _pointDrawerFactory;
        private readonly ILineDrawer _lineDrawer;
        private readonly List<IPointDrawer> _activePointMarkers = new();
        private readonly List<Vector3> _points = new();

        public RouteDrawer(PointDrawerFactory pointDrawerFactory, ILineDrawer lineDrawer, ColorKey color) 
        {
            _pointDrawerFactory = pointDrawerFactory;
            _lineDrawer = lineDrawer;
            _color = color;
        }

        public void AddNode(Vector3 pos, bool isMovable)
        {
            var marker = _pointDrawerFactory.CreateNodePointDrawer(_color, isMovable);
            marker.SetPosition(pos);
            _activePointMarkers.Add(marker);
        }

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

        public void Redraw()
        {
            _lineDrawer.DrawPoints(_points.ToArray());
        }
    }
}
