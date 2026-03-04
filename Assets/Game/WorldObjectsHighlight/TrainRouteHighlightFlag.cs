using UnityEngine;

namespace ZE.NodeStation
{
    public class TrainRouteHighlightFlag : IFlag
    {
        public readonly TimetabledTrain Train;
        public readonly IRoute Route;

        public TrainRouteHighlightFlag(TimetabledTrain train, IRoute route)
        {
            Train = train;
            Route = route;
        }    
    }
}
