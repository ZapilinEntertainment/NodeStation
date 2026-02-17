using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class LaunchTimetabledTrainCommand
    {
        private readonly LaunchTrainCommand _launchCommand;
        private readonly RoutesManager _routesManager;
        private readonly PathsMap _map;

        [Inject]
        public LaunchTimetabledTrainCommand(
            LaunchTrainCommand launchCommand, 
            RoutesManager routesManager,
            PathsMap map)
        {
            _launchCommand = launchCommand;
            _routesManager = routesManager;
            _map = map;
        }

        public void Execute(TimetabledTrain timetabledTrain) 
        {
            timetabledTrain.Status = TimetabledTrainStatus.Launched;
            var train = _launchCommand.Execute(timetabledTrain);

            if (_routesManager.TryGetRoute(timetabledTrain, out var route))
                new RouteTrackController(train, route, _map);

        }

    }
}
