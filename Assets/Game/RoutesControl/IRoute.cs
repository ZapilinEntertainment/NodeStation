using System;
using System.Collections.Generic;

namespace ZE.NodeStation
{
    public interface IRoute : IDisposable
    {
        IReadOnlyList<IPathNode> Points { get; }
        ColorKey ColorKey { get; }

        bool TryGetNextPoint(IPathNode node, out IPathNode nextPoint);


    }
}
