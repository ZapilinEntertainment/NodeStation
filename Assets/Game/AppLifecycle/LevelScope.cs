using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;

namespace ZE.NodeStation
{
    public class LevelScope : LifetimeScope
    {
        [SerializeField] private PathsConstructor _pathsConstructor;
        [SerializeField] private RouteControlsWindow _dragWindow;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private LevelConfig _levelConfig;
        [SerializeField] private TrainsTimetableWindow _timetableWindow;
        [SerializeField] private TimeWindow _timeWindow;
        [Space]
        [Header("app scope:")]
        [SerializeField] private RoutePointDrawer _routePointDrawer;
        [SerializeField] private RouteSegmentLineDrawer _segmentLineDrawer;
        [SerializeField] private ColorPalette _guiColors;
        [SerializeField] private ColorPalette _lightColors;

        private PathsMap _pathsMap;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_cameraController).As<ICameraController>();
            builder.RegisterInstance(_levelConfig);
           
            builder.Register<TickableManager>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
            builder.Register<RailMovementCalculator>(Lifetime.Scoped);

            _pathsMap = _pathsConstructor.ConstructMap();
            builder.RegisterInstance<PathsMap>(_pathsMap);

            builder.Register<TrainFactory>(Lifetime.Scoped);
            builder.Register<RailCarBuilder>(Lifetime.Scoped);

            builder.Register<CollidersManager>(Lifetime.Scoped);

            builder.Register<RouteBuilder>(Lifetime.Scoped);
            builder.Register<RouteChangeController>(Lifetime.Scoped);
            builder.RegisterInstance(_dragWindow);
            builder.Register<GetRouteStartPointCommand>(Lifetime.Scoped);
            builder.Register<RouteDrawManager>(Lifetime.Scoped);
            builder.Register<RouteDrawerFactory>(Lifetime.Scoped);
            builder.Register<RoutesManager>(Lifetime.Scoped);

            builder.Register<LineDrawerFactory>(Lifetime.Scoped);
            builder.Register<PointDrawerFactory>(Lifetime.Scoped);

            builder.Register<TrainsTimetableController>(Lifetime.Scoped);
            builder.Register<TimetabledTrainBuilder>(Lifetime.Scoped);            
            builder.RegisterInstance(_timetableWindow);
            builder.Register<TrainsTimetableWindowController>(Lifetime.Scoped);

            builder.Register<TimeManager>(Lifetime.Scoped);
            builder.RegisterInstance(_timeWindow);
            builder.Register<TimeWindowController>(Lifetime.Scoped);

            builder.Register<LaunchTrainCommand>(Lifetime.Scoped);
            builder.Register<LaunchTimetabledTrainCommand>(Lifetime.Scoped);

            builder.Register<RouteSemaphoresSupervisor>(Lifetime.Scoped);
            builder.Register<PrepareRouteSemaphoresDataCommand>(Lifetime.Scoped);
            builder.Register<SemaphoresManager>(Lifetime.Scoped);
            builder.Register<RouteSemaphoreControllerBuilder>(Lifetime.Scoped);

            builder.RegisterEntryPoint<LevelEntryPoint>(Lifetime.Scoped);  
            
            #if UNITY_EDITOR
            // NOTE: Sometimes produce hidden error and dont dispose!!!
            builder.RegisterDisposeCallback(_ => Debug.Log("level scope disposed"));
            #endif

            // todo: move to app scope
            builder.RegisterInstance<IGUIColorsPalette>(_guiColors);
            builder.RegisterInstance<ILightColorsPalette>(_lightColors);
            PreparePools(builder);

            var messageBroker = MessageBroker.Default;
            builder.RegisterInstance(messageBroker);
        }

        private void PreparePools(IContainerBuilder builder)
        {
            var nodeDrawersPool = new MonoObjectsPool<RoutePointDrawer>(_routePointDrawer);
            builder.RegisterInstance(nodeDrawersPool);

            var lineDrawersPool = new MonoObjectsPool<RouteSegmentLineDrawer>(_segmentLineDrawer);
            builder.RegisterInstance(lineDrawersPool);
        }

        protected override void OnDestroy()
        {
            _pathsMap?.Dispose();
            base.OnDestroy();
        }
    }
}
