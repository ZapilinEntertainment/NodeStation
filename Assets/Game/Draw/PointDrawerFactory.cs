using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class PointDrawerFactory
    {
        private readonly MonoObjectsPool<NodePointDrawer> _nodeDrawers;
        private readonly ColorPalette _colorPalette;

        [Inject]
        public PointDrawerFactory(MonoObjectsPool<NodePointDrawer> nodeDrawers, ColorPalette colorPalette)
        {
            _nodeDrawers = nodeDrawers;
            _colorPalette = colorPalette;
        }

        public IPointDrawer CreateNodePointDrawer(ColorKey key, bool isDraggable)
        {
            var nodeDrawer = _nodeDrawers.Get();
            nodeDrawer.SetDraggable(isDraggable);
            nodeDrawer.SetColor(_colorPalette.GetColor(key));
            return nodeDrawer;
        }
    
    }
}
