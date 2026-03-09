using UnityEngine;

namespace ZE.NodeStation
{
    public interface ITextView
    {
        void SetText(string text);    
        void SetColor(Color color);
        void ResetColor();
    }
}
