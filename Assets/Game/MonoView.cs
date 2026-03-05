using UnityEngine;

namespace ZE.NodeStation
{
    public abstract class MonoView<T> : DisposableMonoBehaviour, IView where T : class, IViewable
    {
        public int ViewId => GetInstanceID();
        protected bool _isOwnerAssigned = false;
        protected T _owner;

        public void AssignOwner(T owner)
        {
            if (IsDisposed)
                return;

            if (_isOwnerAssigned && _owner != null)
            {
                _owner.DisposedEvent -= Dispose;
                _owner.OnViewSet(Constants.NO_VIEW_ID);
            }                

            _owner = owner;
            _isOwnerAssigned = _owner != null;
            if (_isOwnerAssigned)
            {
                _owner.DisposedEvent += Dispose;
                _owner.OnViewSet(ViewId);
            }                
        }

        private void Start()
        {
            DisposeEvent += () => AssignOwner(null);
        }

        private void Update()
        {
            if (!_isOwnerAssigned) return;
            transform.SetPositionAndRotation(_owner.WorldPosition, _owner.WorldRotation);
        }
    }
}
