using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class RouteSemaphoreControllerBuilder
    {
        private readonly TimeManager _timeManager;
        private readonly SemaphoresManager _semaphoresManager;
        private readonly PrepareRouteSemaphoresDataCommand _prepareSemaphoresDataCommand;

        [Inject]
        public RouteSemaphoreControllerBuilder(
            SemaphoresManager semaphoresManager, 
            PrepareRouteSemaphoresDataCommand getAllRouteSemaphoresCommand,
            TimeManager timeManager)
        {
            _semaphoresManager = semaphoresManager;
            _prepareSemaphoresDataCommand = getAllRouteSemaphoresCommand;
            _timeManager = timeManager;
        }

        public RouteSemaphoresController Build(TimetabledTrain train, IRoute route)
        {
            var timeUntilArrival = (train.TrainLaunchTime - _timeManager.CurrentTime).Minutes;

            var trainConfig = train.SpawnInfo.TrainConfiguration;
            var firstBogieOffset = trainConfig.TrainCompositionConfig.GetFirstBogieSpawnOffset();
            var firstBogiePos = firstBogieOffset - timeUntilArrival * trainConfig.MaxSpeed;
            var lastBogiePos = firstBogiePos - firstBogieOffset;     

            return Build(route, firstBogiePos, lastBogiePos);
        }

        public RouteSemaphoresController Rebuild(RouteSemaphoresController previousController, IRoute route) =>
            Build(route, previousController.FirstBogieDist, previousController.LastBogieDist);

        private RouteSemaphoresController Build(IRoute route, float firstBogieDistance, float lastBogieDistance)
        {
            var semaphoresData = _prepareSemaphoresDataCommand.Execute(route);
            var controller = new RouteSemaphoresController(
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
