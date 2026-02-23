using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class PointDrawerFactory
    {
        private readonly MonoObjectsPool<RoutePointDrawer> _draggableDrawers;
        private readonly MonoObjectsPool<RoutePointDrawer> _routePointDrawers;
        private readonly ColorPalette _colorPalette;
        private readonly CollidersManager _collidersManager;

        [Inject]
        public PointDrawerFactory(
            MonoObjectsPool<RoutePointDrawer> nodeDrawers, 
            MonoObjectsPool<RoutePointDrawer> routePointDrawers,    
            ColorPalette colorPalette, 
            CollidersManager collidersManager)
        {
            _draggableDrawers = nodeDrawers;
            _routePointDrawers = routePointDrawers;

            _colorPalette = colorPalette;
            _collidersManager = collidersManager;
        }

        public RoutePointDrawer CreateRoutePointDrawer(TrainRoute route, IPathNode point, int routeIndex, RoutePointMode mode)
        {
            var nodeDrawer = _draggableDrawers.Get();

            nodeDrawer.SetMode(mode);
            nodeDrawer.SetColor(_colorPalette.GetColor(route.ColorKey));
            nodeDrawer.SetPosition(point.WorldPosition);

            switch (mode)
            {
                case RoutePointMode.Draggable:
                    {
                        var controller = new RoutePointDragController(_collidersManager, route, point, nodeDrawer, routeIndex);
                        controller.Initialize();
                        nodeDrawer.DisposeEvent += controller.Dispose;
                        break;
                    }
                    case RoutePointMode.Receiving:
                    {
                        var controller = new RoutePointReceiveController(_collidersManager, point, nodeDrawer);
                        controller.Initialize();
                        nodeDrawer.DisposeEvent += controller.Dispose;
                        break;
                    }
            }

            return nodeDrawer;
        }
    }
}
