using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public class TrainDestinationMarker : FixedWorldSpaceMarker
    {
        private readonly ITextView _textView;

        public TrainDestinationMarker(Vector3 worldPos, Func<Vector3, Vector2> conversionFunc, IWorldMarkerUiView view, TickableManager tickableManager) : 
            base(worldPos, conversionFunc, view, tickableManager)
        {
            _textView = view as ITextView;
        }

        public void Setup(string destinationText) => _textView.SetText(destinationText);
    }
}
