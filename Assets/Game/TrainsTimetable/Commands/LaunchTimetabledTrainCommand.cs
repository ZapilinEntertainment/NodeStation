using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class LaunchTimetabledTrainCommand
    {
        private readonly LaunchTrainCommand _launchCommand;
        private readonly TrainRoutesManager _routesManager;
        private readonly RouteApplyController _routeApplyController;

        [Inject]
        public LaunchTimetabledTrainCommand(
            LaunchTrainCommand launchCommand, 
            TrainRoutesManager routesManager, 
            RouteApplyController routeApplyController)
        {
            _launchCommand = launchCommand;
            _routesManager = routesManager;
            _routeApplyController = routeApplyController;
        }

        public void Execute(TimetabledTrain train) 
        {
            train.Status = TimetabledTrainStatus.Launched;
            _launchCommand.Execute(train);

            if (_routesManager.TryGetRoute(train, out var route))
                _routeApplyController.ApplyRoute(route);

        }

    }
}
