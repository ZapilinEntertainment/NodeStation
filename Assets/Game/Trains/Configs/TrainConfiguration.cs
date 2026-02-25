using UnityEngine;

namespace ZE.NodeStation
{
    [CreateAssetMenu(fileName = nameof(TrainConfiguration), menuName = Constants.ScriptableObjectsFolderPath + nameof(TrainConfiguration))]
    public class TrainConfiguration : ScriptableObject
    {
        [field:SerializeField] public float MaxSpeed { get;private set; } = 1f;
        [field: SerializeField] public float Acceleration { get; private set; } = 1f;
        [field: SerializeField] public float Deceleration { get; private set; } = 0.5f;

        // why base class: there can be some random or variable compositions (opposite to basic fixed ones)
        [field: SerializeField] public TrainCompositionBase TrainCompositionConfig { get; private set; }
    }
}
