using UnityEngine;

namespace ZE.NodeStation
{
    public interface ICameraController
    {
        bool TryRaycastAtCursor(int mask, out RaycastHit rh);
        void SwitchRenderMode(CameraRenderMode mode);
        Vector2 WorldToScreen(Vector3 worldPos);
    
    }
}
