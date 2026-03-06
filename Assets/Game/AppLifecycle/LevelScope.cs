using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;
using ZE.Flags;

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
        [SerializeField] private WorldSpaceMarkersWindow _worldMarkersWindow;
        [Space]
        [Header("app scope:")]
        [SerializeField] private RoutePointDrawer _routePointDrawer;
        [SerializeField] private RouteSegmentLineDrawer _segmentLineDrawer;
        [SerializeField] private ColorPalette _guiColors;
        [SerializeField] private ColorPalette _lightColors;
        [SerializeField] private ColorPalette _materialColors;
        [SerializeField] private HighlightMaterialsPack _highlightMaterialsPack;
        [SerializeField] private WorldSpaceMarkerViewsPack _worldSpaceMarkersPack;

        private PathsMap _pathsMap;

        protected override void Configure(IContainerBuilder builder)
        {
            // TODO: discrete into FeatureInstallers + load prefabs via addresables (path-loading)

            builder.RegisterInstance(_levelConfig);
            builder.Register<ISceneFlagsManager, SceneFlagsManager>(Lifetime.Scoped);
            builder.Register<SceneViewsList>(Lifetime.Scoped);
           
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
            builder.Register<RebuildRouteCommand>(Lifetime.Scoped);
            builder.Register<RouteDrawManager>(Lifetime.Scoped);
            builder.Register<RouteDrawerBuilder>(Lifetime.Scoped);
            builder.Register<RoutesManager>(Lifetime.Scoped);

            builder.Register<LineDrawerFactory>(Lifetime.Scoped);
            builder.Register<PointDrawerFactory>(Lifetime.Scoped);

            builder.Register<TrainsTimetableController>(Lifetime.Scoped).As<ITimetabledTrainsList>().AsSelf();
            builder.Register<TimetabledTrainBuilder>(Lifetime.Scoped);            
            builder.RegisterInstance(_timetableWindow);
            builder.RegisterEntryPoint<TrainsTimetableWindowController>(Lifetime.Scoped);

            builder.RegisterEntryPoint<TrainRouteHighlightEffectController>(Lifetime.Scoped);
            builder.RegisterEntryPoint<TrainHighlightController>(Lifetime.Scoped);

            builder.Register<TimeManager>(Lifetime.Scoped);
            builder.RegisterInstance(_timeWindow);
            builder.Register<TimeWindowController>(Lifetime.Scoped);

            builder.Register<SpawnTrainCommand>(Lifetime.Scoped);
            builder.Register<LaunchTimetabledTrainCommand>(Lifetime.Scoped);

            // # semaphores

            builder.Register<RouteSemaphoresSupervisor>(Lifetime.Scoped);
            builder.Register<PrepareRouteSemaphoresDataCommand>(Lifetime.Scoped);
            builder.Register<SemaphoresManager>(Lifetime.Scoped);
            builder.Register<RouteSemaphoreControllerBuilder>(Lifetime.Scoped);

            // # world space markers
            builder.RegisterInstance(_worldSpaceMarkersPack);
            builder.RegisterInstance<WorldSpaceMarkersWindow, IWorldSpaceMarkersWindow>(_worldMarkersWindow);
            builder.Register<WorldSpaceMarkersFactory>(Lifetime.Scoped);
            builder.Register<WorldSpaceMarkerViewFactory>(Lifetime.Scoped);                  
            builder.RegisterEntryPoint<TrainDestinationsController>(Lifetime.Scoped);
            
            // # start point:

            builder.RegisterEntryPoint<LevelEntryPoint>(Lifetime.Scoped);  
            
#if UNITY_EDITOR
            // NOTE: Sometimes produce hidden error and dont dispose!!!
            builder.RegisterDisposeCallback(_ => Debug.Log("level scope disposed"));
            // TODO: add dispose flag in real build and check if scope was really disposed
#endif

            // todo: move to app scope
            builder.RegisterInstance(_cameraController).As<ICameraController>();
            builder.RegisterInstance<IGUIColorsPalette>(_guiColors);
            builder.RegisterInstance<ILightColorsPalette>(_lightColors);
            builder.RegisterInstance<IMaterialColorsPalette>(_materialColors);
            builder.RegisterInstance(_highlightMaterialsPack);
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
