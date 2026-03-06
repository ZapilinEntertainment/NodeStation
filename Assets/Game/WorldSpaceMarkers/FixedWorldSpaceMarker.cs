using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public class FixedWorldSpaceMarker : WorldSpaceMarker
    {
        protected override Vector3 WorldPos => _worldPos;
        private readonly Vector3 _worldPos;        

        public FixedWorldSpaceMarker(Vector3 worldPos, Func<Vector3, Vector2> conversionFunc, IWorldMarkerUiView view, TickableManager tickableManager) : 
            base(conversionFunc, view, tickableManager)
        {
            _worldPos = worldPos;
        }        
    }
}
