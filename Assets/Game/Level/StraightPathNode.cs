using UnityEngine;

namespace ZE.NodeStation
{
    public class StraightPathNode : AbstractNode
    {
        public readonly int ExitNodeKey;

        public StraightPathNode(int key, int entranceNodeKey, int exitNodeKey) : base(key, entranceNodeKey)
        {
            ExitNodeKey = exitNodeKey;
        }

        public override bool TryGetExitNode(int entranceNodeKey, out int exitNodeKey)
        {
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
    }
}
