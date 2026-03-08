using UnityEngine;

namespace ZE.NodeStation
{
    public enum RouteStatus
    {
        Undefined, Missed, Correct
    
    }

    public static class RouteStatusExtension
    {
        public static float GetColorSaturation(this RouteStatus status) => status == RouteStatus.Correct ? 1f : 0.5f;
    }
}
