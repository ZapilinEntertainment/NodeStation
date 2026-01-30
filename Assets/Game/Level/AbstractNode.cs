using UnityEngine;

namespace ZE.NodeStation
{
    public abstract class AbstractNode : IPathNode
    {
        public readonly int Key;
        public readonly int EntranceNodeKey;

        public AbstractNode(int key, int entranceNodeKey)
        {
            Key = key;
            EntranceNodeKey = entranceNodeKey;
        }

        public abstract bool TryGetExitNode(int entranceNodeKey, out int exitNodeKey);
    }
}
