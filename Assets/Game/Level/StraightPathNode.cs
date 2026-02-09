using UnityEngine;

namespace ZE.NodeStation
{
    public class StraightPathNode : AbstractNode
    {
        public readonly int ExitNodeKey;
        public override NodeType Type => NodeType.Straight;
        public override bool IsFinal => false;

        public override bool HaveMultipleExits => false;

        public StraightPathNode(Vector3 worldPos, int key, int entranceNodeKey, int exitNodeKey, NodeFunction function) : 
            base(worldPos, key, entranceNodeKey, function)
        {
            ExitNodeKey = exitNodeKey;
        }

        public override bool TryGetExitNode(int entranceNodeKey, out int exitNodeKey)
        {
            //Debug.Log($"requested: {entranceNodeKey}, existing: entrance - {EntranceNodeKey}, exit - {ExitNodeKey}");
            if (EntranceNodeKey == entranceNodeKey)
            {
                exitNodeKey = ExitNodeKey;
                return true;
            }

            if (entranceNodeKey == ExitNodeKey)
            {
                exitNodeKey = EntranceNodeKey;
                return true;
            }

            exitNodeKey = Constants.NO_EXIT_PATH_CODE;
            return false;
        }

        public override bool TrySetupPath(int entranceNodeKey, int exitNodeKey) => 
            (entranceNodeKey == EntranceNodeKey && ExitNodeKey == exitNodeKey) 
            || (entranceNodeKey == exitNodeKey && ExitNodeKey == EntranceNodeKey);
    }
}
