using System;
using UnityEngine;

namespace ZE.NodeStation
{
    // notice: when creating new implements, add to NodeBuilder
    public interface IPathNode : IDisposable
    {
        NodeType Type { get; }
        bool IsFinal { get; }
        Vector3 WorldPosition { get; }
        event Action DisposeEvent;
        int Key { get; }

        // try switch rails to make path between entrance and exit
        bool TrySetupPath(int entranceNodeKey, int exitNodeKey);

        // get current exit, that can be reached from this entrance by default (without switching)
        bool TryGetExitNode(int entranceNodeKey, out int exitNodeKey);

        // get all available exits from this entrance
        // note that it is not same as ask map to have all connections -
        // results may vary by entrance key (ex.: ask division path node from 2-sides or from 1-side)
        NodeExitsContainer GetAllExits(int entranceNodeKey);
    }
}
