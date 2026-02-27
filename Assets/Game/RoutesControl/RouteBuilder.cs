using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UniRx;

namespace ZE.NodeStation
{
    public class RouteBuilder
    {
        private readonly PathsMap _map;
        private readonly IMessageBroker _messageBroker;

        [Inject]
        public RouteBuilder(PathsMap pathsMap, IMessageBroker messageBroker)
        {
            _map = pathsMap;
            _messageBroker = messageBroker;
        }

        // just get next point until path ends
        public bool TryBuildRoute(int startNode, ColorKey colorKey, out RouteController route)
        {
            var points = new List<IPathNode>();
            var nodeKey = startNode;
            var nextNodeFound = false;
            var prevNodeKey = Constants.NO_EXIT_PATH_CODE;

            if (!_map.TryGetNode(nodeKey, out var node))
            {
                Debug.LogError("Route spawn node incorrect");
                route = default;
                return false;
            }

            do
            {
                points.Add(node);

                nextNodeFound = node.TryGetExitNode(prevNodeKey, out var nextNodeKey);
                if (!nextNodeFound)
                {
                    //Debug.Log($"stop route: ${prevNodeKey} -> {nextNodeKey}");
                    break;
                }
                    

                prevNodeKey = nodeKey;
                nodeKey = nextNodeKey;       

                if (!_map.TryGetNode(nodeKey, out node))
                {
                    Debug.LogError($"node {nodeKey} not exists");
                    break;
                }
            }
            while (nextNodeFound);

            route = new RouteController(_messageBroker, new TrainRoute(colorKey, points));
            return true;
        }
    }
}
