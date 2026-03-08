using UnityEngine;

namespace ZE.NodeStation
{
    public class DestinationLabelMarkerPoint : MonoBehaviour
    {
        [field:SerializeField] public int DestinationIndex { get;private set; }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(transform.position, DebugConstants.DESTINATION_MARKER_SIZE);
            Gizmos.DrawSphere(transform.TransformPoint(DebugConstants.DESTINATION_MARKER_SIZE * 2f * Vector3.forward), DebugConstants.DESTINATION_MARKER_SIZE * 0.5f);
        }
#endif
    }
}
