using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class RouteDrawerFactory
    {
        private readonly LineDrawerFactory _lineDrawerFactory;
        private readonly PointDrawerFactory _pointDrawerFactory;

        [Inject]
        public RouteDrawerFactory(LineDrawerFactory lineDrawerFactory, PointDrawerFactory pointDrawerFactory)
        {
            _lineDrawerFactory = lineDrawerFactory;
            _pointDrawerFactory = pointDrawerFactory;
        }

        public RouteDrawer Create(TrainRoute route)
        {
            var colorKey = route.ColorKey;

            return new RouteDrawer(
                _pointDrawerFactory,
                _lineDrawerFactory.CreateRouteLineDrawer(colorKey),
                colorKey);
        }
    }
}
