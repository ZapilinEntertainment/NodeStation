using UnityEngine;
using Unity.Mathematics;

namespace ZE.NodeStation
{
    public class RailPathBase : MonoBehaviour
    {
        [SerializeField] private Transform _start;
        [SerializeField] private Transform _end;
        [SerializeField] private RailPathBase _startConnectedRail;
        [SerializeField] private RailPathBase _endConnectedRail;
        

#if UNITY_EDITOR

        virtual protected void DrawGizmos()
        {
            if (_start != null && _end != null)
            {
                var start = _start.position;
                var end = _end.position;
                Gizmos.DrawLine(start, end);
                Gizmos.DrawSphere(start, DebugConstants.RAIL_NODES_RADIUS);
                Gizmos.DrawSphere(end, DebugConstants.RAIL_NODES_RADIUS);

                var dir = Vector3.Normalize(end - start);
                var center = Vector3.Lerp(start, end, 0.5f);
                Gizmos.DrawLine(center, Quaternion.AngleAxis(135f, Vector3.up) * dir + center);
                Gizmos.DrawLine(center, Quaternion.AngleAxis(135f, Vector3.down) * dir + center);
            }
        }
#endif
    }
}
