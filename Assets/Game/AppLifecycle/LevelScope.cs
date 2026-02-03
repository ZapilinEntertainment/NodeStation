using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ZE.NodeStation
{
    public class LevelScope : LifetimeScope
    {
        [SerializeField] private PathsConstructor _pathsConstructor;
        private PathsMap _pathsMap;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<TickableManager>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
            builder.Register<RailMovementCalculator>(Lifetime.Scoped);

            _pathsMap = _pathsConstructor.ConstructMap();
            builder.RegisterInstance<PathsMap>(_pathsMap);

            builder.Register<TrainFactory>(Lifetime.Scoped);
            builder.Register<RailCarBuilder>(Lifetime.Scoped);
        }

        protected override void OnDestroy()
        {
            _pathsMap?.Dispose();
            base.OnDestroy();
        }
    }
}
