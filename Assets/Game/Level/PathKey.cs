using System;
using UnityEngine;

namespace ZE.NodeStation
{
    [Serializable]
    public struct PathKey : IEquatable<PathKey>
    {
        public int StartNodeKey;
        public int EndNodeKey;

        // order is not important
        public bool Equals(PathKey other) => 
            ((StartNodeKey == other.StartNodeKey) &&( EndNodeKey == other.EndNodeKey))
            ||
            ((StartNodeKey == other.EndNodeKey) && (EndNodeKey == other.StartNodeKey));

        public PathKey(int startNodeKey, int endNodeKey)
        {
            StartNodeKey = startNodeKey;
            EndNodeKey = endNodeKey;
        }

        public PathKey(IPathNode startNode, IPathNode endNode)
        {
            StartNodeKey = startNode.Key;
            EndNodeKey = endNode.Key;
        }

        public override string ToString() => $"({StartNodeKey}->{EndNodeKey})";
    }
}
