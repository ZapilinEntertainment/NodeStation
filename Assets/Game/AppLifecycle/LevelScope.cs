using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ZE.NodeStation
{
    public class LevelScope : LifetimeScope
    {
        [SerializeField] private PathsConstructor _pathsConstructor;
        [SerializeField] private RouteControlsWindow _dragWindow;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private LevelConfig _levelConfig;
        [SerializeField] private TrainsTimetableWindow _timetableWindow;
        [Space]
        [Header("app scope:")]
        [SerializeField] private RoutePointDrawer _nodePointDrawer;
        [SerializeField] private RouteSegmentLineDrawer _segmentLineDrawer;
        [SerializeField] private ColorPalette _colorPalette;
        [SerializeField] private SwitchableRoutePoint _switchableRoutePointPrefab;

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
            builder.Register<RouteApplyController>(Lifetime.Scoped);
            builder.Register<AddSwitchablePointsReceiverCommand>(Lifetime.Scoped);
            builder.RegisterInstance(_dragWindow);
            builder.Register<GetRouteStartPointCommand>(Lifetime.Scoped);
            builder.Register<RouteDrawManager>(Lifetime.Scoped);
            builder.Register<RouteDrawerFactory>(Lifetime.Scoped);

            builder.Register<LineDrawerFactory>(Lifetime.Scoped);
            builder.Register<PointDrawerFactory>(Lifetime.Scoped);

            builder.RegisterEntryPoint<TrainsTimetableController>(Lifetime.Scoped);
            builder.Register<TimetabledTrainBuilder>(Lifetime.Scoped);
            builder.Register<TrainsTimetableWindow>(Lifetime.Scoped);
            builder.RegisterInstance(_timetableWindow);

            builder.RegisterEntryPoint<LevelEntryPoint>(Lifetime.Scoped);

           

            // todo: move to app scope
            builder.RegisterInstance(_colorPalette);
            PreparePools(builder);
        }

        private void PreparePools(IContainerBuilder builder)
        {
            var nodeDrawersPool = new MonoObjectsPool<RoutePointDrawer>(_nodePointDrawer);
            builder.RegisterInstance(nodeDrawersPool);

            var lineDrawersPool = new MonoObjectsPool<RouteSegmentLineDrawer>(_segmentLineDrawer);
            builder.RegisterInstance(lineDrawersPool);

            builder.RegisterInstance(_switchableRoutePointPrefab);
        }

        protected override void OnDestroy()
        {
            _pathsMap?.Dispose();
            base.OnDestroy();
        }
    }
}
