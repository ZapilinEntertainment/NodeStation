using UnityEngine;

namespace ZE.NodeStation
{
    public abstract class AbstractNode : IPathNode
    {
        public readonly int Key;
        public readonly int EntranceNodeKey;
        public readonly NodeFunction NodeFunction;
        public Vector3 WorldPosition => _worldPos;
        public abstract NodeType Type { get; }
        public abstract bool IsFinal { get;}

        public abstract bool HaveMultipleExits { get; }

        protected readonly Vector3 _worldPos;

        public AbstractNode(Vector3 worldPos, int key, int entranceNodeKey, NodeFunction nodeFunction)
        {
            _worldPos = worldPos;
            Key = key;
            EntranceNodeKey = entranceNodeKey;
            NodeFunction = nodeFunction;
        }

        public abstract bool TryGetExitNode(int entranceNodeKey, out int exitNodeKey);

        public abstract bool TrySetupPath(int entranceNodeKey, int exitNodeKey);
    }
}
