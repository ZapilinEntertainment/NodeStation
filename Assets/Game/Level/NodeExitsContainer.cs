using UnityEngine;

namespace ZE.NodeStation
{
    // up to 3 exits
    public struct NodeExitsContainer
    {
        public int Count;
        public int ExitA;
        public int ExitB;
        public int ExitC;

        public static readonly NodeExitsContainer Empty = new NodeExitsContainer()
        {
            Count = 0,
            ExitA = Constants.NO_EXIT_PATH_CODE,
            ExitB = Constants.NO_EXIT_PATH_CODE,
            ExitC = Constants.NO_EXIT_PATH_CODE
        };
    
        public NodeExitsContainer(params int[] exits)
        {
            Count = Mathf.Clamp( exits.Length,0, 3);
            switch(Count)
            {
                case 1:
                    {
                        ExitA = exits[0];
                        ExitB = Constants.NO_EXIT_PATH_CODE;
                        ExitC = Constants.NO_EXIT_PATH_CODE;
                        break;
                    }
                case 2:
                    {
                        ExitA = exits[0];
                        ExitB = exits[1];
                        ExitC = Constants.NO_EXIT_PATH_CODE;
                        break;
                    }
                case 3:
                    {
                        ExitA = exits[0];
                        ExitB = exits[1];
                        ExitC = exits[2];
                        break;
                    }
                default:
                    {
                        ExitA = Constants.NO_EXIT_PATH_CODE;
                        ExitB = Constants.NO_EXIT_PATH_CODE;
                        ExitC = Constants.NO_EXIT_PATH_CODE;
                        break;
                    }
            }
        }

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return ExitA;
                    case 1: return ExitB;
                    case 2: return ExitC;
                    default: return Constants.NO_EXIT_PATH_CODE;
                }
            }
        }
    }
}
