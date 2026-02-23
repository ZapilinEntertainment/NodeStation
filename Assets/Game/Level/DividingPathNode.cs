using UnityEngine;

namespace ZE.NodeStation
{
    public class DividingPathNode : AbstractNode
    {
        // scheme:
        //                 / ExitNodeKeyA
        // EntranceNodeKey - ExitNodeKeyB


        public readonly int ExitNodeKeyA;
        public readonly int ExitNodeKeyB;
        public bool UsePathB;
        public override bool IsFinal => false;
        public override NodeType Type => _type;

        protected readonly NodeType _type;

        public DividingPathNode(Vector3 worldPos, int key, int entranceKey, int exitKeyA, int exitKeyB, bool isReversed, NodeFunction function) : 
            base(worldPos, key, entranceKey, function)
        {
            ExitNodeKeyA = exitKeyA;
            ExitNodeKeyB = exitKeyB;
            _type = isReversed ? NodeType.DividingReversed : NodeType.Dividing;
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

        public override bool TrySetupPath(int entranceNodeKey, int exitNodeKey)
        {
            // enters from single line:

            if (exitNodeKey == EntranceNodeKey)
            {
                if (entranceNodeKey == ExitNodeKeyA)
                {
                    UsePathB = false;
                    return true;
                }

                if (entranceNodeKey == ExitNodeKeyB)
                {
                    UsePathB = true;
                    return true;
                }

                return false;
            }

            // enters from divided lines:

            if (entranceNodeKey != EntranceNodeKey)
                return false;

            if (exitNodeKey == ExitNodeKeyA)
            {
                UsePathB = false;
                return true;
            }

            if (exitNodeKey == ExitNodeKeyB)
            {
                UsePathB = true;
                return true;
            }

            return false;
        }

        public override NodeExitsContainer GetAllExits(int entranceNodeKey)
        {
            if (entranceNodeKey == EntranceNodeKey)
                return new(ExitNodeKeyA, ExitNodeKeyB);

            if (entranceNodeKey == ExitNodeKeyA || entranceNodeKey == ExitNodeKeyB)
                return new(EntranceNodeKey);

            return NodeExitsContainer.Empty;
        }
    }
}
