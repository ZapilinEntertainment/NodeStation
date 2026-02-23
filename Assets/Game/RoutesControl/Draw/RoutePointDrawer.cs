using System;
using UnityEngine;
using UnityEngine.Pool;

namespace ZE.NodeStation
{
    public class RoutePointDrawer : WorldSpriteDrawer, IPointDrawer, IPoolable<RoutePointDrawer>
    {

        [SerializeField] private MonoPropertySwitcher _propertiesSwitcher;
        [SerializeField] private Collider _dragCollider;
        [SerializeField] private Collider _receiveCollider;

        private IObjectPool<RoutePointDrawer> _pool;

        public void AssignToPool(IObjectPool<RoutePointDrawer> pool) => _pool = pool;
        public void SetMode(RoutePointMode mode) => _propertiesSwitcher.SwitchState((int)mode);

        public Collider DragCollider => _dragCollider;
        public Collider ReceiveCollider => _receiveCollider;
        public event Action DisposeEvent;

        public void Dispose()
        {
            if (IsDestroyed)
                return;

            if (_pool != null)
                _pool.Release(this);
            else
                FinalDispose();
        }

        public void FinalDispose()
        {
            if (IsDestroyed)
                return;

            _pool = null;
            Destroy(gameObject);
        }

        public void OnGet() 
        { 
            gameObject.SetActive(true);
        }

        public void OnRelease() 
        { 
            DisposeEvent?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
