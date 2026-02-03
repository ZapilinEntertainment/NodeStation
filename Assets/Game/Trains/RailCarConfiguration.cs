using UnityEngine;

namespace ZE.NodeStation
{
    [CreateAssetMenu(fileName = nameof(RailCarConfiguration), menuName = Constants.ScriptableObjectsFolderPath + nameof(RailCarConfiguration))]
    public class RailCarConfiguration : ScriptableObject
    {
        [field: SerializeField] public RailCarView Prefab { get; private set; }
        [field: SerializeField] public float FrontBogeyOffset { get;private set; } = 1f;
        [field: SerializeField] public float RearBogeyOffset { get; private set; } = -1f;
        [field: SerializeField] public float CarLength { get; private set; } = 10f;

    }
}
