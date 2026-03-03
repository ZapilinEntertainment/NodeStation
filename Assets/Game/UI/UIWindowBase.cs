using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public abstract class UIWindowBase : DisposableMonoBehaviour
    {
        protected event Action WindowHideEvent;

        private void Start()
        {
            DisposeEvent += OnDispose;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (IsDisposed)
                return; 
            WindowHideEvent?.Invoke();
            if (gameObject != null)
                gameObject.SetActive(false);
        }

        private void OnDispose()
        {
            WindowHideEvent = null;
        }
    }
}
