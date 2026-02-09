using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class PointDrawerFactory
    {
        private readonly MonoObjectsPool<RoutePointDrawer> _nodeDrawers;
        private readonly ColorPalette _colorPalette;
        private readonly CollidersManager _collidersManager;
        private readonly SwitchableRoutePoint _routeReceiverPrefab;

        [Inject]
        public PointDrawerFactory(
            MonoObjectsPool<RoutePointDrawer> nodeDrawers, 
            ColorPalette colorPalette, 
            CollidersManager collidersManager,
            SwitchableRoutePoint routeReceiverPrefab)
        {
            _nodeDrawers = nodeDrawers;
            _colorPalette = colorPalette;
            _collidersManager = collidersManager;
            _routeReceiverPrefab = routeReceiverPrefab;
        }

        public RoutePointDrawer CreateRoutePointDrawer(TrainRoute route, IPathNode point, bool isDraggable, int routeIndex)
        {
            var nodeDrawer = _nodeDrawers.Get();

            nodeDrawer.SetDraggable(isDraggable);
            nodeDrawer.SetColor(_colorPalette.GetColor(route.ColorKey));
            nodeDrawer.SetPosition(point.WorldPosition);

            if (isDraggable)
            {
                var controller = new RoutePointController(_collidersManager, route, point, nodeDrawer, routeIndex);
                nodeDrawer.DisposeEvent += controller.Dispose;
            }

            return nodeDrawer;
        }

        public SwitchableRoutePoint CreateSwitchableRoutePoint(IPathNode point)
        {
            var receiver = GameObject.Instantiate(_routeReceiverPrefab);
            receiver.transform.position = point.WorldPosition;
            receiver.AssignNode(point);
            var colliderKey = _collidersManager.Register(receiver);

            point.DisposeEvent += () =>
            {
                receiver?.Dispose();
                _collidersManager?.Unregister(colliderKey);
            };

            return receiver;
        }

    }
}
