using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UniRx;

namespace ZE.NodeStation
{
    // create new drawer for each call
    // drawers wont be changed in no way  -  they will be re-created completely
    public class RouteDrawManager : IDisposable
    {
        private readonly Dictionary<IRoute, RouteDrawer> _drawers = new();
        private readonly RouteDrawerBuilder _routeDrawerBuilder;
        private readonly CompositeDisposable _compositeDisposable = new();

        [Inject]
        public RouteDrawManager(RouteDrawerBuilder routeDrawerBuilder, IMessageBroker messageBroker)
        {
            _routeDrawerBuilder = routeDrawerBuilder;

            messageBroker
                .Receive<RouteDisposedMessage>()
                .Subscribe(OnRouteDisposed)
                .AddTo(_compositeDisposable);

            messageBroker
                .Receive<RouteChangedMessage>()
                .Subscribe(OnRouteUpdated)
                .AddTo(_compositeDisposable);
        }        

        public void DrawRoute(IRoute route, bool redraw = false)
        {
            if (_drawers.ContainsKey(route))
            {
                if (redraw)
                    ClearRouteDrawing(route);
                else
                    return;
            }
            var drawer = _routeDrawerBuilder.Build(route);
            _drawers.Add(route, drawer);
        }

        public void ClearRouteDrawing(IRoute route)
        {
            if (_drawers.TryGetValue(route, out var drawer))
            {
                drawer?.Dispose();
                _drawers.Remove(route);
            }
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();

            foreach (var drawer in _drawers.Values)
                drawer.Dispose();

            _drawers.Clear();
        }

        private void OnRouteDisposed(RouteDisposedMessage msg) => ClearRouteDrawing(msg.Route);
        private void OnRouteUpdated(RouteChangedMessage msg)
        {
            if (_drawers.ContainsKey(msg.Route))
                DrawRoute(msg.Route, true);
        }
    }
}
