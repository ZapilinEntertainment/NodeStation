using UnityEngine;

namespace ZE.NodeStation
{
    public class DividingPathNode : AbstractNode
    {
        public readonly int ExitNodeKeyA;
        public readonly int ExitNodeKeyB;
        public bool UsePathB;

        public DividingPathNode(int key, int entranceKey, int exitKeyA, int exitKeyB) : base(key, entranceKey)
        {
            ExitNodeKeyA = exitKeyA;
            ExitNodeKeyB = exitKeyB;
        }

        public override bool TryGetExitNode(int entranceNodeKey, out int exitNodeKey)
        {
            if (entranceNodeKey == EntranceNodeKey)
            {
                exitNodeKey = UsePathB ? ExitNodeKeyB : ExitNodeKeyA;
                return true;
            }

            if (entranceNodeKey == ExitNodeKeyA || entranceNodeKey == ExitNodeKeyB)
            {
                exitNodeKey = EntranceNodeKey;
                return true;
            }

            exitNodeKey = Constants.NO_EXIT_PATH_CODE;
            return false;
        }
    }
}
