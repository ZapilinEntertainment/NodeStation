using UnityEngine;
using TMPro;

namespace ZE.NodeStation
{
    public class TextWorldSpaceMarkerView : WorldSpaceMarkerUiView, ITextView
    {
        [SerializeField] private TextMeshProUGUI _label;

        public void SetText(string text) => _label.text = text;
    }
}
