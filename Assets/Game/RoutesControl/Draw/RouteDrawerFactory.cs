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
                var canBeControlledByPlayer = false;
                NodeExitsContainer exits = default;
                
                // previous point have more than 1 suitable exit
                // note: we need entrance of previous point (thats why > 1, not > 0)
                if (i > 1)
                {
                    exits = points[i - 1].GetAllExits(points[i - 2].Key);
                    canBeControlledByPlayer = exits.Count > 1;
                }

                var node = points[i];
                if (canBeControlledByPlayer)
                {
                    for (var j = 0; j < exits.Count; j++)
                    {
                        var exitNodeKey = exits[j];
                        if (node.Key == exitNodeKey)
                        {
                            // current selected exit
                            var pointDrawer = _pointDrawerFactory.CreateRoutePointDrawer(route, node, i, RoutePointMode.Draggable);
                            drawer.AddNodeDrawer(pointDrawer);
                        }
                        else
                        {
                            if (!_map.TryGetNode(exitNodeKey, out var otherExitNode)) 
                                continue;
                            // other possible exits
                            var pointDrawer = _pointDrawerFactory.CreateRoutePointDrawer(route, otherExitNode, i, RoutePointMode.Receiving);
                            drawer.AddNodeDrawer(pointDrawer);
                        }
                    }
                }
                else
                {
                    var pointDrawer = _pointDrawerFactory.CreateRoutePointDrawer(route, node,i, RoutePointMode.Default);
                    drawer.AddNodeDrawer(pointDrawer);
                }

                // TODO: add other segment draw options
                drawer.AddLinePoint(node.WorldPosition);
            }
            drawer.FinishDraw();
            return drawer;
        }
    }
}
