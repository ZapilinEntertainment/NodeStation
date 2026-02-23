using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public abstract class RoutePointController : IDisposable, IColliderOwner
    {
        public IPathNode Node => _node;

        private readonly CollidersManager _collidersManager;
        private readonly IPathNode _node;
        private int _colliderKey;

        public RoutePointController(CollidersManager collidersManager, IPathNode node)
        {
            _collidersManager = collidersManager;
            _node = node;
            
        }

        public void Initialize() => _colliderKey = _collidersManager.Register(this);

        public void Dispose() => _collidersManager?.Unregister(_colliderKey);

        public abstract int GetColliderId();
    }
}
