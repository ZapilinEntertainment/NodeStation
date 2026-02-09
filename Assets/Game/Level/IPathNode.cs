using System;
using UnityEngine;

namespace ZE.NodeStation
{
    // notice: when creating new implements, add to NodeBuilder
    public interface IPathNode : IDisposable
    {
        bool TryGetExitNode(int entranceNodeKey, out int exitNodeKey);   
        NodeType Type { get; }
        bool IsFinal { get; }
        bool HaveMultipleExits { get; }
        Vector3 WorldPosition { get; }
        event Action DisposeEvent;
        int Key { get; }

        bool TrySetupPath(int entranceNodeKey, int exitNodeKey);
    }
}
