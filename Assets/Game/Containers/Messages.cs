using UnityEngine;

namespace ZE.NodeStation
{
    public class TrainAnnouncedMessage
    {
        public readonly TimetabledTrain Train;
        public TrainAnnouncedMessage(TimetabledTrain train) => Train = train;
    }

    public class RouteDisposedMessage
    {
        public readonly RouteController RouteController;
        public IRoute Route => RouteController.Route;
        public RouteDisposedMessage(RouteController controller) => RouteController = controller;
    }

    public class RouteChangedMessage
    {
        public readonly RouteController RouteController;
        public IRoute Route => RouteController.Route;
        public RouteChangedMessage(RouteController controller) => RouteController = controller;
    }
}
