using UnityEngine;

namespace ZE.NodeStation
{
    public class LimitedTextWorldSpaceMarkerView : TextWorldSpaceMarkerView
    {
        public override void SetPosition(Vector2 screenPos)
        {
            if (IsDisposed)
                return;

            var halfwidth = RectTransform.rect.width * 0.5f;
            var halfheight = RectTransform.rect.height * 0.5f;

            screenPos.x = Mathf.Clamp(screenPos.x, halfwidth, Screen.width - halfwidth);
            screenPos.y = Mathf.Clamp(screenPos.y, halfheight, Screen.height - halfheight);

            transform.position = screenPos;
        }
    }
}
