using UnityEngine;
using TMPro;

namespace ZE.NodeStation
{
    public class TextWorldSpaceMarkerView : WorldSpaceMarkerUiView, ITextView
    {
        [SerializeField] private TextMeshProUGUI _label;
        private Color? _cachedColor;

        public void ResetColor()
        {
            _label.color = _cachedColor ?? Color.white;
        }

        public void SetColor(Color color)
        {
            _cachedColor ??= _label.color;
            _label.color = color;
        }

        public void SetText(string text) => _label.text = text;
    }
}
