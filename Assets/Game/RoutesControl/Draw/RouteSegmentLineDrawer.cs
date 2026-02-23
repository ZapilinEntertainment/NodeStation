using UnityEngine;
using UnityEngine.Pool;

namespace ZE.NodeStation
{
    public class RouteSegmentLineDrawer : MonoBehaviour, ILineDrawer, IPoolable<RouteSegmentLineDrawer>
    {
        [SerializeField] private LineRenderer _lineRenderer;
        private bool _isDestroyed = false;
        private IObjectPool<RouteSegmentLineDrawer> _pool;

        public void SetColor(Color color)
        {
            _lineRenderer.startColor = color;
            _lineRenderer.endColor = color;
        }

        public void DrawPoints(Vector3[] positions) 
        {
            _lineRenderer.positionCount = positions.Length;            
            _lineRenderer.SetPositions(positions);
        }

        public void AssignToPool(IObjectPool<RouteSegmentLineDrawer> pool) => _pool = pool;

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
            GameObject.Destroy(gameObject);
        }

        public void OnGet() 
        { 
            gameObject.SetActive(true);
        }

        public void OnRelease()
        {
            gameObject.SetActive(false);
            _lineRenderer.SetPositions(new Vector3[0]);
        }

        private void OnDestroy() => _isDestroyed = true;
    }
}
