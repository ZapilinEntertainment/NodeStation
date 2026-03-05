using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public class DisposableMonoBehaviour : MonoBehaviour, IDisposable
    {
        public event Action DisposeEvent;
        protected bool IsDisposed { get;private set;} = false;
        

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
