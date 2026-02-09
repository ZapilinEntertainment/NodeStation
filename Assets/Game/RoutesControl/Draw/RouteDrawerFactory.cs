using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class RouteDrawerFactory
    {
        private readonly LineDrawerFactory _lineDrawerFactory;
        private readonly PointDrawerFactory _pointDrawerFactory;
        private readonly PathsMap _map;

        [Inject]
        public RouteDrawerFactory(LineDrawerFactory lineDrawerFactory, PointDrawerFactory pointDrawerFactory, PathsMap map)
        {
            _lineDrawerFactory = lineDrawerFactory;
            _pointDrawerFactory = pointDrawerFactory;
            _map = map;
        }

        public RouteDrawer Create(TrainRoute route)
        {
            var colorKey = route.ColorKey;

            var drawer = new RouteDrawer(
                _lineDrawerFactory.CreateRouteLineDrawer(colorKey),
                colorKey);

            var points = route.Points;
            for (var i = 0; i < points.Count; i++)
            {
                var isNotFirstPoint = i != 0;
                
                var canBeControlledByPlayer =
                    isNotFirstPoint
                    && points[i-1].HaveMultipleExits;

                var node = points[i];
                drawer.AddNodeDrawer(_pointDrawerFactory.CreateRoutePointDrawer(route, node, canBeControlledByPlayer, i));

                // TODO: add other segment draw options
                drawer.AddLinePoint(node.WorldPosition);
            }
            drawer.FinishDraw();
            return drawer;
        }
    }
}
