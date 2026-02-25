using UnityEngine;
using VContainer;
using UniRx;

namespace ZE.NodeStation
{
    public class LaunchTimetabledTrainCommand
    {
        private readonly LaunchTrainCommand _launchCommand;
        private readonly RoutesManager _routesManager;
        private readonly PathsMap _map;
        private readonly IMessageBroker _messageBroker;

        [Inject]
        public LaunchTimetabledTrainCommand(
            LaunchTrainCommand launchCommand, 
            RoutesManager routesManager,
            PathsMap map,
            IMessageBroker messageBroker)
        {
            _launchCommand = launchCommand;
            _routesManager = routesManager;
            _map = map;
            _messageBroker = messageBroker;
        }

        public void Execute(TimetabledTrain timetabledTrain) 
        {
            var train = _launchCommand.Execute(timetabledTrain);

            if (_routesManager.TryGetRoute(timetabledTrain, out var route))
            {
                new RouteTrackController(train, route, _map).Init();
                _messageBroker.Publish<TrainAnnouncedMessage>(new(timetabledTrain));
            }

            timetabledTrain.OnTrainLaunched(train);
        }

    }
}
