using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace ZE.NodeStation
{
    public class AddSwitchablePointsReceiverCommand 
    {
        private readonly PathsMap _map;
        private readonly PointDrawerFactory _pointsDrawerFactory;

        public AddSwitchablePointsReceiverCommand(PathsMap map, PointDrawerFactory pointDrawerFactory)
        {
            _map = map;
            _pointsDrawerFactory = pointDrawerFactory;
        }
    
        public void Execute()
        {
            var entrances = new Dictionary<int,int>();
            var exits = new Dictionary<int, int>();
            var paths = _map.Paths.Keys;

            // calculate exits and entrances per each node
            foreach (var path in paths)
            {
                if (!entrances.TryGetValue(path.EndNodeKey, out var entrancesCount))
                    entrancesCount = 0;

                entrances[path.EndNodeKey] = entrancesCount + 1;

                if (!exits.TryGetValue(path.StartNodeKey, out var exitsCount))
                    exitsCount = 0;

                exits[path.StartNodeKey] = exitsCount + 1;
            }

            var switchables = new HashSet<int>();

            // for nodes with 2 or more entrances add all its entrances
            foreach (var entrancesKvp in entrances)
            {
                var entrancesCount = entrancesKvp.Value;
                if (entrancesCount < 2)
                    continue;

                var exit = entrancesKvp.Key;
                var counter = 0;
                foreach (var path in paths)
                {
                    if (path.EndNodeKey != exit)
                        continue;

                    switchables.Add(path.StartNodeKey);
                    counter++;
                    if (counter == entrancesCount)
                        break;
                }
            }

            // for nodes with 2 or more exits add all its exits
            foreach (var exitsKvp in exits)
            {
                var exitsCount = exitsKvp.Value;
                if (exitsCount < 2)
                    continue;

                var entrance = exitsKvp.Key;
                var counter = 0;

                foreach (var path in paths)
                {
                    if (path.StartNodeKey != entrance)
                        continue;

                    switchables.Add(path.EndNodeKey);
                    counter++;
                    if (counter == exitsCount)
                        break;
                }
            }

            entrances.Clear();
            exits.Clear();

            foreach (var switchable in switchables)
            {
                _map.TryGetNode(switchable, out var node);
                _pointsDrawerFactory.CreateSwitchableRoutePoint(node);
            }
        }
    }
}
