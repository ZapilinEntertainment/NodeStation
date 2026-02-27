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
        private readonly RouteDrawManager _routeDrawManager;
        private readonly RebuildRouteCommand _rebuildRouteCommand;

        private const int DRAGGABLES_MASK = LayerMasks.USER_DRAGGABLE_MASK;
        private const int DRAGGABLES_RECEIVERS_MASK = LayerMasks.DRAGGABLES_RECEIVERS_MASK;

        private bool _isMovingRoutePoint = false;
        private IDraggableRoutePoint _movingRoutePoint = null;

        [Inject]
        public RouteChangeController(
            RouteControlsWindow window, 
            ICameraController cameraController, 
            CollidersManager collidersManager,
            RouteDrawManager routeDrawManager,
            RebuildRouteCommand rebuildRouteCommand)
        {
            _window = window;
            _cameraController = cameraController;
            _collidersManager = collidersManager;
            _routeDrawManager = routeDrawManager;
            _rebuildRouteCommand = rebuildRouteCommand;

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
                && _collidersManager.TryIdentifyColliderAs<IReceivingRoutePoint>(rh.colliderInstanceID, out var routePoint)
                && _rebuildRouteCommand.TryExecute(_movingRoutePoint, routePoint.Node))
            {
                // all updates will call automatically
                // there may be some success update effects (ex.: sound)
            }

            _isMovingRoutePoint = false;
            _movingRoutePoint = null;
        }
    }
}
