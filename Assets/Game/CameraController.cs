using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ZE.NodeStation
{
    public class CameraController : MonoBehaviour, ICameraController
    {
        [SerializeField] private Camera _camera;
        private UniversalAdditionalCameraData _cameraData;

        public Camera Camera => _camera;

        public void SwitchRenderMode(CameraRenderMode mode)
        {
            _cameraData = _camera.GetComponent<UniversalAdditionalCameraData>();
            _cameraData.SetRenderer((int)mode);
        }

        public bool TryRaycastAtCursor(int mask, out RaycastHit rh)
        {
            var pos = Input.mousePosition;
            var ray = _camera.ScreenPointToRay(pos);
            var hit =  Physics.Raycast(ray, maxDistance: Constants.MAX_INPUT_RAYCAST_LENGTH, layerMask: mask, hitInfo: out rh);
            return hit;
        }
    }
}
