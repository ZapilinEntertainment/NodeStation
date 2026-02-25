using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class CalculateSemaphoreDistancesCommand
    {
        private readonly PathsMap _map;

        [Inject]
        public CalculateSemaphoreDistancesCommand(PathsMap map)
        {
            _map = map;
        }

        public float[] Execute(IReadOnlyList<SemaphoreDecoration> semaphores)
        {
            var count = semaphores.Count;
            if (count < 2)
                return new float[0];

            var distances = new float[count-1];
            for (var i = 0; i < count - 1; i++)
            {
                if (!_map.TryGetPath(semaphores[i].MapPosition.Path, out var path))
                {
                    Debug.LogWarning("wrong semaphore path!");
                    continue;
                }
                    
                distances[i] = path.Length;
            }

            return distances;
        }
    }
}
