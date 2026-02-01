using UnityEngine;

namespace ZE.NodeStation
{
    public class DeadEndNode : AbstractNode
    {
        public override NodeType Type => NodeType.DeadEnd;

        public DeadEndNode(int key, int entranceNodeKey) : base(key, entranceNodeKey)
        {
        }

        public override bool TryGetExitNode(int entranceNodeKey, out int exitNodeKey)
        {
            exitNodeKey = Constants.NO_EXIT_PATH_CODE;
            return false;
        }
    }
}
