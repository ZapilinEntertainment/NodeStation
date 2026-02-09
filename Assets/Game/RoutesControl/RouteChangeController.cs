using System;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    // handles user-operating route changes
    public class RouteChangeController : IDisposable
    {
        private readonly ICameraController _cameraController;
        private readonly RouteControlsWindow _window;
        private readonly CollidersManager _collidersManager;
        private readonly RouteBuilder _routeBuilder;

        private const int DRAGGABLES_MASK = LayerMasks.USER_DRAGGABLE_MASK;
        private const int DRAGGABLES_RECEIVERS_MASK = LayerMasks.DRAGGABLES_RECEIVERS_MASK;

        private bool _isMovingRoutePoint = false;
        private IDraggableRoutePoint _movingRoutePoint = null;

        [Inject]
        public RouteChangeController(
            RouteControlsWindow window, 
            ICameraController cameraController, 
            CollidersManager collidersManager,
            RouteBuilder routeBuilder)
        {
            _window = window;
            _cameraController = cameraController;
            _collidersManager = collidersManager;
            _routeBuilder = routeBuilder;

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

            if (_cameraController.TryRaycastAtCursor(DRAGGABLES_MASK, out var rh)
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
                && _collidersManager.TryIdentifyColliderAs<IRoutePoint>(rh.colliderInstanceID, out var routePoint))
            {
                _routeBuilder.TryRebuildRoute(_movingRoutePoint, routePoint);
            }

            _isMovingRoutePoint = false;
            _movingRoutePoint = null;
        }
    }
}
