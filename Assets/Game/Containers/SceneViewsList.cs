using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    public class SceneViewsList : IDisposable
    {
        private readonly Dictionary<int, IView> _views = new();

        public void Dispose()
        {
            foreach (var view in _views.Values)
            {
                if (view == null)
                    continue;
                view.Dispose();
            }
            _views.Clear();
        }

        public void RegisterView(IView view)
        {
            var key = view.GetInstanceID();
            _views.Add(key, view);
            view.DisposeEvent += () => _views.Remove(key);
        }

        public bool TryGetView(int viewKey, out IView view) => _views.TryGetValue(viewKey, out view);
    }
}
