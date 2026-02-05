using UnityEngine;

namespace ZE.NodeStation
{
    public class DividingPathNode : AbstractNode
    {
        public readonly int ExitNodeKeyA;
        public readonly int ExitNodeKeyB;
        public bool UsePathB;
        public override bool IsFinal => false;
        public override NodeType Type => _type;
        protected readonly NodeType _type;

        public DividingPathNode(int key, int entranceKey, int exitKeyA, int exitKeyB, bool isReversed, NodeFunction function) : base(key, entranceKey, function)
        {
            ExitNodeKeyA = exitKeyA;
            ExitNodeKeyB = exitKeyB;
            _type = isReversed ? NodeType.DividingReversed : NodeType.Dividing;
        }

        // TODO: there is problem where multi-bogie trains enters the inverted division and its tail suddenly appears on other line

        public override bool TryGetExitNode(int entranceNodeKey, out int exitNodeKey)
        {
            //Debug.Log($"requested: {entranceNodeKey} of {EntranceNodeKey}, {ExitNodeKeyA}, {ExitNodeKeyB}");

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
