using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public abstract class AbstractNode : IPathNode
    {      
        public readonly int EntranceNodeKey;
        public readonly NodeFunction NodeFunction;
        public Vector3 WorldPosition => _worldPos;
        public abstract NodeType Type { get; }
        public abstract bool IsFinal { get;}
        public int Key => _key;
        public event Action DisposeEvent;

        protected readonly Vector3 _worldPos;
        private readonly int _key;

        public override string ToString() => $"node {_key}";

        public AbstractNode(Vector3 worldPos, int key, int entranceNodeKey, NodeFunction nodeFunction)
        {
            _worldPos = worldPos;
            _key = key;
            EntranceNodeKey = entranceNodeKey;
            NodeFunction = nodeFunction;
        }

        public abstract bool TryGetExitNode(int entranceNodeKey, out int exitNodeKey);

        public abstract bool TrySetupPath(int entranceNodeKey, int exitNodeKey);
        public abstract NodeExitsContainer GetAllExits(int entranceNodeKey);

        public void Dispose() => DisposeEvent?.Invoke();    
    }
}
