using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class LineDrawerFactory
    {
        private readonly ColorPalette _colorPalette;
        private readonly MonoObjectsPool<RouteSegmentLineDrawer> _segmentsPool;

        [Inject]
        public LineDrawerFactory(ColorPalette colorPalette, MonoObjectsPool<RouteSegmentLineDrawer> segmentsPool)
        {
            _colorPalette = colorPalette;
            _segmentsPool = segmentsPool;
        }

        public ILineDrawer CreateRouteLineDrawer(ColorKey colorKey)
        {
            var segmentDrawer = _segmentsPool.Get();
            segmentDrawer.SetColor(_colorPalette.GetColor(colorKey));
            return segmentDrawer;
        }
    
    }
}
