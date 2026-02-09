using UnityEngine;

namespace ZE.NodeStation
{
    public class DeadEndNode : AbstractNode
    {
        public override NodeType Type => NodeType.DeadEnd;

        public override bool IsFinal => NodeFunction == NodeFunction.Spawn || NodeFunction == NodeFunction.Exit;

        public override bool HaveMultipleExits => false;

        public DeadEndNode(Vector3 worldPos, int key, int entranceNodeKey, NodeFunction function) : base(worldPos, key, entranceNodeKey, function)
        {
        }

        public override bool TryGetExitNode(int entranceNodeKey, out int exitNodeKey)
        {
            if (entranceNodeKey == EntranceNodeKey)
            {
                exitNodeKey = Constants.NO_EXIT_PATH_CODE;
                return false;
            }
            else
            {
                exitNodeKey = EntranceNodeKey;
                return true;
            }
        }

        public override bool TrySetupPath(int entranceNodeKey, int exitNodeKey) => exitNodeKey == EntranceNodeKey || entranceNodeKey == EntranceNodeKey;
    }
}
