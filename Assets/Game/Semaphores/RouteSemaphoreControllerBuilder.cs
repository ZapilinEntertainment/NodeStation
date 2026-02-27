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

            var firstBogiePos = CalculateFirstBogiePos(train);
            var timeUntilArrival = (_timeManager.CurrentTime - train.TrainLaunchTime).Seconds;
            var lastBogieDistance = timeUntilArrival * train.SpawnInfo.TrainConfiguration.MaxSpeed * -1f;
            var firstBogieDistance = lastBogieDistance + firstBogiePos;
            

            var controller =  new RouteSemaphoresController(
                route: route, 
                semaphoresManager: _semaphoresManager,
                semaphoresData: semaphoresData,
                igniteDistance: Constants.SEMAPHORE_IGNITE_DISTANCE,
                extinguishDistance: Constants.SEMAPHORE_EXTINGUISH_DISTANCE,
                firstBogieDistance: firstBogiePos,
                lastBogieDistance: lastBogieDistance);

#if UNITY_EDITOR
            var debugDrawer = GameObject.FindAnyObjectByType<DEBUG_RouteTrainBogiesDrawer>();
            debugDrawer?.SetupRoute(route, controller);
#endif

            return controller;
        }

        private float CalculateFirstBogiePos(TimetabledTrain train)
        {
            var compositionConfig = train.SpawnInfo.TrainConfiguration.TrainCompositionConfig;

            // last bogie pos is 0
            return compositionConfig.CalculateTrainLength() - compositionConfig.GetRearOverhang() - compositionConfig.GetFrontOverhang();
        }
    
    }
}
