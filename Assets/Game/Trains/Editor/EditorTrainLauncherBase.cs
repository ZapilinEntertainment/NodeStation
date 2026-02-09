using UnityEngine;
using VContainer;
using TriInspector;

namespace ZE.NodeStation
{
    public abstract class EditorTrainLauncherBase : MonoBehaviour
    {
        [Space]
        [SerializeField] protected TrainConfiguration _trainConfig;
        [SerializeField] protected RailCarBuildProtocol[] _railCars;
        [SerializeField] protected float _speedPercent = 0f;
        [SerializeField] protected bool _isAccelerating = false;

        protected TrainFactory _trainFactory;
        protected PathsMap _map;

        [Inject]
        public void Inject(TrainFactory trainFactory, PathsMap map)
        {
            _trainFactory = trainFactory;
            _map = map;
        }

        [InfoBox("Available only in Playmode")]
        [Button("Spawn train"), EnableInPlayMode]
        public virtual void LaunchTrain()
        {
            var train = _trainFactory.Build(_trainConfig, GetSpawnPosition(), _railCars);
            train.SetSpeed(_speedPercent, _isAccelerating);
        }

        protected abstract RailPosition GetSpawnPosition(); 
    }
}
