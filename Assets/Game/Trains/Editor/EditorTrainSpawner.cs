using UnityEngine;
using TriInspector;
using VContainer;

namespace ZE.NodeStation
{
    public class EditorTrainSpawner : MonoBehaviour
    {
        [SerializeField] private MapPosition _launchPosition;
        [SerializeField] private TrainConfiguration _trainConfig;
        [SerializeField] private float _speedPercent = 0f;
        [SerializeField] private bool _isAccelerating = false;

        private TrainFactory _trainFactory;
        private PathsMap _map;

        [Inject]
        public void Inject(TrainFactory trainFactory, PathsMap map)
        {
            _trainFactory = trainFactory;
            _map = map;
        }

        [Button("Spawn train")]
        public void LaunchTrain()
        {
            if (!_map.TryGetPath(_launchPosition.Path, out var path))
            {
                Debug.LogError("Invalid launch path");
                return;
            }

            var position = path.GetPosition(_launchPosition.Percent);
            position.IsReversed = _launchPosition.IsReversed;

            var train = _trainFactory.Build(_trainConfig, position);
            train.SetSpeed(_speedPercent, _isAccelerating);
        }
    }
}
