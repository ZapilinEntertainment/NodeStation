using UnityEngine;

namespace ZE.NodeStation
{
    public enum TimetabledTrainStatus : byte { NotReady, Announced, Launched, CompletedRoute, Disposed }

    public static class TimetabledTrainStatusExtension
    {
        public static bool CanChangeRoute(this TimetabledTrainStatus status) => status == TimetabledTrainStatus.Announced;
    }
}
