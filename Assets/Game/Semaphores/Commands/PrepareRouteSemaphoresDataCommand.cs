using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class PrepareRouteSemaphoresDataCommand
    {
        private readonly SemaphoresManager _semaphoresList;
        private readonly PathsMap _pathsMap;

        [Inject]
        public PrepareRouteSemaphoresDataCommand(SemaphoresManager semaphoresList, PathsMap pathsMap)
        {
            _semaphoresList = semaphoresList;
            _pathsMap = pathsMap;
        }

        public List<RouteSemaphoreData> Execute(TrainRoute route)
        {
            var list = new List<RouteSemaphoreData>();
            var points = route.Points;
            if (points.Count < 2)
            {
                Debug.LogWarning("invalid route points count!");
                return list;
            }
                
            var sumDistance = 0f;
            for (var i = 1; i < points.Count; i++)
            {
                var currentPointKey = points[i].Key;
                var pathKey = new PathKey(points[i-1].Key, currentPointKey);
                if (_semaphoresList.TryGetSemaphore(pathKey, out var semaphore) && _pathsMap.TryGetPath(pathKey, out var path))
                {
                    var dist = path.Length * semaphore.MapPosition.Percent;

                    list.Add(new()
                    {
                        Distance = sumDistance + dist,
                        Semaphore = semaphore,
                        IsFront = path.PathKey.EndNodeKey == currentPointKey
                    });

                    sumDistance += path.Length;
                }
                   
            }

            return list;
        }
    
    }
}
