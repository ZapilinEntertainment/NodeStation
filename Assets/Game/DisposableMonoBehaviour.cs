using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public class DisposableMonoBehaviour : MonoBehaviour, IDisposable
    {
        protected bool IsDisposed { get;private set;} = false;
        protected event Action DisposeEvent;

        public void Dispose()
        {
            if (!IsDisposed)
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            IsDisposed = true;
            DisposeEvent?.Invoke();
            DisposeEvent = null;
        }
    }
}
