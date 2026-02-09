using UnityEngine;

namespace ZE.NodeStation
{
    public interface ICameraController
    {
        bool TryRaycastAtCursor(int mask, out RaycastHit rh);
    
    }
}
