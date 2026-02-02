using UnityEngine;

namespace ZE.NodeStation
{
    public abstract class AbstractNode : IPathNode
    {
        public readonly int Key;
        public readonly int EntranceNodeKey;
        public readonly NodeFunction NodeFunction;
        public abstract NodeType Type { get; }
        public abstract bool IsFinal { get;}

        public AbstractNode(int key, int entranceNodeKey, NodeFunction nodeFunction)
        {
            Key = key;
            EntranceNodeKey = entranceNodeKey;
            NodeFunction = nodeFunction;
        }

        public abstract bool TryGetExitNode(int entranceNodeKey, out int exitNodeKey);
    }
}
