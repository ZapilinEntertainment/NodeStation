using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public class TrainView : MonoBehaviour, ITrainView, IDisposable
    {
        private bool _isOwnerAssigned = false;
        private bool _isDisposed = false;
        private ITrain _train;

        public void AssignOwner(ITrain train)
        {
            if (_isOwnerAssigned) 
                _train.DisposedEvent -= Dispose;

            _train = train;
            _isOwnerAssigned = _train != null;
            if (_isOwnerAssigned)
                _train.DisposedEvent += Dispose;
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            if (_isOwnerAssigned)
                _train.DisposedEvent -= Dispose;
            _train = null;
            _isOwnerAssigned = false;

            Destroy(gameObject);
        }

        private void Update()
        {
            if (!_isOwnerAssigned) return;
            transform.SetPositionAndRotation(_train.WorldPosition, _train.WorldRotation);
        }

        // for editor calls
        private void OnDestroy() => Dispose();
    }
}
