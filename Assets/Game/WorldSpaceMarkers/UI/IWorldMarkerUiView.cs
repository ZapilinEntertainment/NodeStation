using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public interface IWorldMarkerUiView : IDisposable
    {
        void SetPosition(Vector2 screenPos);
    
    }
}
