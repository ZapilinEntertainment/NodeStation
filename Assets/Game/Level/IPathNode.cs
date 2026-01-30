using UnityEngine;

namespace ZE.NodeStation
{
    // notice: when creating new implements, add to NodeBuilder
    public interface IPathNode
    {
        bool TryGetExitNode(int entranceNodeKey, out int exitNodeKey);    
    }
}
