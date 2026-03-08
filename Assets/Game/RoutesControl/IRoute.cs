using System;
using System.Collections.Generic;
using UniRx;

namespace ZE.NodeStation
{
    public interface IRoute : IDisposable
    {
        RouteStatus Status => StatusProperty.Value;

        IReadOnlyList<IPathNode> Points { get; }
        IReadOnlyReactiveProperty<RouteStatus> StatusProperty { get; }
        ColorKey ColorKey { get; }

        bool TryGetNextPoint(IPathNode node, out IPathNode nextPoint);


    }
}
