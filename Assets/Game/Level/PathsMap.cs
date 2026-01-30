using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    public class PathsMap : IDisposable
    {
        private readonly Dictionary<int, IPathNode> _nodes = new();
        private readonly Dictionary<PathKey, IRailPath> _paths = new();

        public void AddNode(int key, IPathNode node) => _nodes.Add(key, node);
        public void AddPath(in PathKey key, IRailPath path) => _paths.Add(key, path);

        public bool TryGetNextRail(IRailPath currentRail, out IRailPath nextRail, bool reversedMovement = false)
        {
            var pathKey = currentRail.RegistrationKey;
            if (!_paths.ContainsKey(pathKey))
            {
                Debug.LogError("Rail not registered!");
                nextRail = currentRail;
                return false;
            }

            var currentNodeKey = reversedMovement ? pathKey.StartNodeKey : pathKey.EndNodeKey;
            var prevNodeKey = reversedMovement ? pathKey.EndNodeKey : pathKey.StartNodeKey;
            if (!_nodes.TryGetValue(currentNodeKey, out var nextNode))
            {
                Debug.LogError($"Node {currentNodeKey} not found!");
                nextRail = currentRail;
                return false;
            }

            if (!nextNode.TryGetExitNode(prevNodeKey, out var exitNodeKey))
            {
                // train crush
                nextRail = currentRail;
                return false;
            }

            pathKey = new PathKey(currentNodeKey, exitNodeKey) ;
            if (!_paths.TryGetValue(pathKey, out nextRail))
            {
                Debug.LogError("Invalid node exit path!");
                nextRail = currentRail;
                return false;
            }

            return true;
        }

        public bool TryGetPath(in PathKey key, out IRailPath path) => _paths.TryGetValue(key, out path);

        public void Dispose()
        {
            _nodes.Clear();
            _paths.Clear();
        }
    
    }
}
