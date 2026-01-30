using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public readonly struct PathKey : IEquatable<PathKey>
    {
        public readonly int StartNodeKey;
        public readonly int EndNodeKey;

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
    }
}
