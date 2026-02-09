using UnityEngine;
using TriInspector;

namespace ZE.NodeStation
{
    public enum NodeFunction : byte { None, Spawn, Exit }
    public enum NodeType : byte { Undefined, Straight, DeadEnd, Dividing, DividingReversed }

    public class ConstructingNodePoint : MonoBehaviour
    {
        // todo: specific triggers or node abilities
        [field: SerializeField] public NodeFunction Function { get; private set; }

        [DisplayAsString] public NodeType Type;
    }
}
