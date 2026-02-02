using UnityEngine;

namespace ZE.NodeStation
{
    public class DeadEndNode : AbstractNode
    {
        public override NodeType Type => NodeType.DeadEnd;

        public override bool IsFinal => NodeFunction == NodeFunction.Spawn || NodeFunction == NodeFunction.Exit;

        public DeadEndNode(int key, int entranceNodeKey, NodeFunction function) : base(key, entranceNodeKey, function)
        {
        }

        public override bool TryGetExitNode(int entranceNodeKey, out int exitNodeKey)
        {
            exitNodeKey = Constants.NO_EXIT_PATH_CODE;
            return false;
        }
    }
}
