using UnityEngine;

namespace ZE.NodeStation
{
    [CreateAssetMenu(fileName = nameof(TrainConfiguration), menuName = Constants.ScriptableObjectsFolderPath + nameof(TrainConfiguration))]
    public class TrainConfiguration : ScriptableObject
    {
        [field:SerializeField] public float MaxSpeed { get;private set; } = 1f;
        [field: SerializeField] public float Acceleration { get; private set; } = 1f;
        [field: SerializeField] public float Deceleration { get; private set; } = 0.5f;
        
    }
}
