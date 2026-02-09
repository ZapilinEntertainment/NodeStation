using UnityEngine;
using TriInspector;
using VContainer;

namespace ZE.NodeStation
{
    public class EditorTrainSpawner : EditorTrainLauncherBase
    {
        [SerializeField] private MapPosition _launchPosition;

        protected override RailPosition GetSpawnPosition()
        {
            if (!_map.TryGetPath(_launchPosition.Path, out var path))
            {
                Debug.LogError("Invalid launch path");
                return default;
            }

            return path.GetPosition(_launchPosition.Percent, _launchPosition.IsReversed);
        }
    }
}
