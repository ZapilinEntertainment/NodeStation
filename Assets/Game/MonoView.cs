using UnityEngine;

namespace ZE.NodeStation
{
    public abstract class MonoView<T> : MonoBehaviour where T : class, IViewable
    {
        protected bool _isOwnerAssigned = false;
        protected bool _isDisposed = false;
        protected T _owner;

        public void AssignOwner(T owner)
        {
            if (_isOwnerAssigned)
                _owner.DisposedEvent -= Dispose;

            _owner = owner;
            _isOwnerAssigned = _owner != null;
            if (_isOwnerAssigned)
            {
                _owner.DisposedEvent += Dispose;
                OnOwnerAssigned();
            }
                
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            if (_isOwnerAssigned)
                _owner.DisposedEvent -= Dispose;
            _owner = null;
            _isOwnerAssigned = false;

            Destroy(gameObject);
        }

        protected virtual void OnOwnerAssigned() { }

        private void Update()
        {
            if (!_isOwnerAssigned) return;
            transform.SetPositionAndRotation(_owner.WorldPosition, _owner.WorldRotation);
        }

        // for editor calls
        private void OnDestroy() => Dispose();
    }
}
