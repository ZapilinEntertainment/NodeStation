using UnityEngine;

namespace ZE.NodeStation
{
    public class CameraController : MonoBehaviour, ICameraController
    {
        [SerializeField] private Camera _camera;

        public bool TryRaycastAtCursor(int mask, out RaycastHit rh)
        {
            var pos = Input.mousePosition;
            var ray = _camera.ScreenPointToRay(pos);
            return Physics.Raycast(ray, maxDistance: Constants.MAX_INPUT_RAYCAST_LENGTH, layerMask: mask, hitInfo: out rh);
        }
    }
}
