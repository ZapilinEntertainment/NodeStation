using UnityEngine;
using UnityEngine.Pool;

namespace ZE.NodeStation
{
    public class NodePointDrawer : MonoBehaviour, IPointDrawer, IPoolable<NodePointDrawer>
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Collider _dragCollider;
        [SerializeField] private Collider _receiveCollider;
        private IObjectPool<NodePointDrawer> _pool;
        private bool _isDestroyed = false;

        public void AssignToPool(IObjectPool<NodePointDrawer> pool) => _pool = pool;
        public void SetDraggable(bool x) => _dragCollider.enabled = x;
        public void SetColor(Color color) => _spriteRenderer.color = color;
        public void SetPosition(Vector3 pos) => transform.position = pos;

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

        public void OnGet() 
        { 
            _receiveCollider.enabled = true;
        }

        public void OnRelease() 
        { 
            SetDraggable(false); 
            _receiveCollider.enabled = false;
        }

        private void OnDestroy() => _isDestroyed = true;
    }
}
