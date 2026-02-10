using System;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    // handles user-operating route changes
    public class RouteChangeController : IDisposable
    {
        // TODO: rework to events \ messages

        private readonly ICameraController _cameraController;
        private readonly RouteControlsWindow _window;
        private readonly CollidersManager _collidersManager;
        private readonly RouteBuilder _routeBuilder;
        private readonly RouteDrawManager _routeDrawManager;
        private readonly RouteApplyController _routeApplyController;

        private const int DRAGGABLES_MASK = LayerMasks.USER_DRAGGABLE_MASK;
        private const int DRAGGABLES_RECEIVERS_MASK = LayerMasks.DRAGGABLES_RECEIVERS_MASK;

        private bool _isMovingRoutePoint = false;
        private IDraggableRoutePoint _movingRoutePoint = null;

        [Inject]
        public RouteChangeController(
            RouteControlsWindow window, 
            ICameraController cameraController, 
            CollidersManager collidersManager,
            RouteBuilder routeBuilder,
            RouteDrawManager routeDrawManager,
            RouteApplyController routeApplyController)
        {
            _window = window;
            _cameraController = cameraController;
            _collidersManager = collidersManager;
            _routeBuilder = routeBuilder;
            _routeDrawManager = routeDrawManager;
            _routeApplyController = routeApplyController;

            _window.DragStartEvent += OnBeginDrag;
            _window.DragEndEvent += OnEndDrag;
        }

        public void Dispose()
        {
            if (_window != null)
            {
                _window.DragStartEvent -= OnBeginDrag;
                _window.DragEndEvent -= OnEndDrag;
            }
        }

        private void OnBeginDrag()
        {
            if (_isMovingRoutePoint)
                return;

            if (_cameraController.TryRaycastAtCursor(int.MaxValue, out var rh)
                && _collidersManager.TryIdentifyColliderAs<IDraggableRoutePoint>(rh.colliderInstanceID, out var routePoint))
            {
                _isMovingRoutePoint = true;
                _movingRoutePoint = routePoint;
            }
        }

        private void OnEndDrag()
        {
            if (!_isMovingRoutePoint)
                return;

            if (_cameraController.TryRaycastAtCursor(DRAGGABLES_RECEIVERS_MASK, out var rh)
                && _collidersManager.TryIdentifyColliderAs<SwitchableRoutePoint>(rh.colliderInstanceID, out var routePoint)
                && _routeBuilder.TryRebuildRoute(_movingRoutePoint, routePoint.Node))
            {
                var route = _movingRoutePoint.Route;
                _routeDrawManager.ClearRouteDrawing(route);
                _routeDrawManager.DrawRoute(route);
                _routeApplyController.ApplyRoute(route);
            }

            _isMovingRoutePoint = false;
            _movingRoutePoint = null;
        }
    }
}
