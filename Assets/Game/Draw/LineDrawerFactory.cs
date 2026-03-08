using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class LineDrawerFactory
    {
        private readonly IGUIColorsPalette _colorPalette;
        private readonly MonoObjectsPool<RouteSegmentLineDrawer> _segmentsPool;

        [Inject]
        public LineDrawerFactory(IGUIColorsPalette colorPalette, MonoObjectsPool<RouteSegmentLineDrawer> segmentsPool)
        {
            _colorPalette = colorPalette;
            _segmentsPool = segmentsPool;
        }

        public ILineDrawer CreateRouteLineDrawer(ColorKey colorKey, float saturation)
        {
            var segmentDrawer = _segmentsPool.Get();
            var baseColor = _colorPalette.GetGUIColor(colorKey);
            segmentDrawer.SetColor(Color.Lerp(Color.grey, baseColor, saturation));
            return segmentDrawer;
        }
    
    }
}
