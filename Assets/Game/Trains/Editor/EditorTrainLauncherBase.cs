using UnityEngine;
using VContainer;
using TriInspector;

namespace ZE.NodeStation
{
    public abstract class EditorTrainLauncherBase : MonoBehaviour
    {
        [Space]
        [SerializeField] protected TrainConfiguration _trainConfig;
        [SerializeField] protected float _speedPercent = 0f;
        [SerializeField] protected bool _isAccelerating = false;
        protected PathsMap _map;

        private SpawnTrainCommand _launchTrainCommand;

        [Inject]
        public void Inject(PathsMap map, SpawnTrainCommand launchTrainCommand)
        {
            _map = map;
            _launchTrainCommand = launchTrainCommand;
        }

        [InfoBox("Available only in Playmode")]
        [Button("Spawn train"), EnableInPlayMode]
        public virtual void LaunchTrain()
        {
            var spawnPosition = GetSpawnPosition();
            _launchTrainCommand.Execute(_trainConfig, spawnPosition, _speedPercent, _isAccelerating);
        }

        protected abstract RailPosition GetSpawnPosition(); 
    }
}
