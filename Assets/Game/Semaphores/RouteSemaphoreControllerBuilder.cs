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
            var semaphoresData = _prepareSemaphoresDataCommand.Execute(route);

            var timeUntilArrival = (train.TrainLaunchTime - _timeManager.CurrentTime).Minutes;

            var trainConfig = train.SpawnInfo.TrainConfiguration;
            var firstBogieOffset = trainConfig.TrainCompositionConfig.GetFirstBogieSpawnOffset();
            var firstBogiePos = firstBogieOffset - timeUntilArrival * trainConfig.MaxSpeed;
            var lastBogiePos = firstBogiePos - firstBogieOffset;
            

            var controller =  new RouteSemaphoresController(
                route: route, 
                semaphoresManager: _semaphoresManager,
                semaphoresData: semaphoresData,
                igniteDistance: Constants.SEMAPHORE_IGNITE_DISTANCE,
                extinguishDistance: Constants.SEMAPHORE_EXTINGUISH_DISTANCE,
                firstBogieDistance: firstBogiePos,
                lastBogieDistance: lastBogiePos);

#if UNITY_EDITOR
            var debugDrawer = GameObject.FindAnyObjectByType<DEBUG_RouteTrainBogiesDrawer>();
            debugDrawer?.SetupRoute(route, controller);
#endif

            return controller;
        }
    
    }
}
