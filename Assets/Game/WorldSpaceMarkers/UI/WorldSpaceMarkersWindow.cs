using UnityEngine;

namespace ZE.NodeStation
{
    public class WorldSpaceMarkersWindow : MonoBehaviour, IWorldSpaceMarkersWindow
    {
        [SerializeField] private Transform _markersHost;

        public Transform MarkersHost => _markersHost;
    }
}
