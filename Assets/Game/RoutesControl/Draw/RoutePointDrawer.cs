using System;
using UnityEngine;
using UnityEngine.Pool;

namespace ZE.NodeStation
{
    public class RoutePointDrawer : MonoBehaviour, IPointDrawer, IPoolable<RoutePointDrawer>
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Collider _dragCollider;
        private IObjectPool<RoutePointDrawer> _pool;
        private bool _isDestroyed = false;

        public void AssignToPool(IObjectPool<RoutePointDrawer> pool) => _pool = pool;
        public void SetDraggable(bool x) => _dragCollider.enabled = x;
        public void SetColor(Color color) => _spriteRenderer.color = color;
        public void SetPosition(Vector3 pos) => transform.position = pos;
        public Collider DragCollider => _dragCollider;
        public event Action DisposeEvent;

        public void Dispose()
        {
            if (_isDestroyed)
                return;

            if (_pool != null)
                _pool.Release(this);
            else
                FinalDispose();
        }

        public void FinalDispose()
        {
            if (_isDestroyed)
                return;

            _pool = null;
            Destroy(gameObject);
        }

        public void OnGet() { }

        public void OnRelease() 
        { 
            SetDraggable(false); 
            DisposeEvent?.Invoke();
        }

        private void OnDestroy() => _isDestroyed = true;
    }
}
