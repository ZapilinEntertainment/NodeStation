using System;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class WorldSpaceMarkerViewFactory
    {
        private readonly IWorldSpaceMarkersWindow _worldSpaceMarkersWindow;
        private readonly WorldSpaceMarkerViewsPack _prefabsPack;      

        [Inject]
        public WorldSpaceMarkerViewFactory(IWorldSpaceMarkersWindow window, WorldSpaceMarkerViewsPack _prefabsPack)
        {
            _worldSpaceMarkersWindow = window;
            this._prefabsPack = _prefabsPack;
        }

        // TODO: need pooling?
        public IWorldMarkerUiView CreateView(WorldSpaceMarkerViewKey key)
        {
            if (!_prefabsPack.TryGetPrefab(key, out var prefab)) 
                return null;

            var view = GameObject.Instantiate(prefab, _worldSpaceMarkersWindow.MarkersHost);
            return view;
        }
    }
}
