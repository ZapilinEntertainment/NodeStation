using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ZE.NodeStation
{
    public class LevelScope : LifetimeScope
    {
        [SerializeField] private PathsConstructor _pathsConstructor;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<RailMovementCalculator>(Lifetime.Scoped);

            var map = _pathsConstructor.ConstructMap();
            builder.RegisterInstance<PathsMap>(map);
        }
    }
}
