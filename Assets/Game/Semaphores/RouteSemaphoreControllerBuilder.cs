using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class RouteSemaphoreControllerBuilder
    {
        private readonly TimeManager _timeManager;
        private readonly RoutesManager _routesManager;
        private readonly SemaphoresManager _semaphoresManager;
        private readonly PrepareRouteSemaphoresDataCommand _prepareSemaphoresDataCommand;

        [Inject]
        public RouteSemaphoreControllerBuilder(
            SemaphoresManager semaphoresManager, 
            PrepareRouteSemaphoresDataCommand getAllRouteSemaphoresCommand,
            RoutesManager routesManager,
            TimeManager timeManager)
        {
            _semaphoresManager = semaphoresManager;
            _prepareSemaphoresDataCommand = getAllRouteSemaphoresCommand;
            _routesManager = routesManager;
            _timeManager = timeManager;
        }

        public RouteSemaphoresController Build(TimetabledTrain train)
        {
            if (!_routesManager.TryGetRoute(train, out var route))
            {
                Debug.LogError("timetabled train have no route");
                return null;
            }

            var timeUntilArrival = (_timeManager.CurrentTime - train.TrainLaunchTime).Seconds;
            var trainConfig = train.SpawnInfo.TrainConfiguration;
            var firstBogieDistance = timeUntilArrival * trainConfig.MaxSpeed * -1f;
            var lastBogieDistance = firstBogieDistance - trainConfig.TrainCompositionConfig.CalculateTrainLength();

            var semaphoresData = _prepareSemaphoresDataCommand.Execute(route);

            var controller =  new RouteSemaphoresController(
                route: route, 
                semaphoresManager: _semaphoresManager,
                semaphoresData: semaphoresData,
                igniteDistance: Constants.SEMAPHORE_IGNITE_DISTANCE,
                extinguishDistance: Constants.SEMAPHORE_EXTINGUISH_DISTANCE,
                firstBogieDistance: firstBogieDistance,
                lastBogieDistance: lastBogieDistance);

#if UNITY_EDITOR
            var debugDrawer = GameObject.FindAnyObjectByType<DEBUG_RouteTrainBogiesDrawer>();
            debugDrawer?.SetupRoute(route, controller);
#endif

            return controller;
        }
    
    }
}
