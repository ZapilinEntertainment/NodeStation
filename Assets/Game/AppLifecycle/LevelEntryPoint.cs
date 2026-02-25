using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ZE.NodeStation
{
    public class LevelEntryPoint : IStartable
    {
        private readonly IObjectResolver _resolver;

        [Inject]
        public LevelEntryPoint(IObjectResolver resolver) 
        {
            _resolver = resolver;
        }

        public void Start()
        {
            _resolver.Resolve<RouteChangeController>();

            _resolver.Resolve<TimeManager>();
            _resolver.Resolve<TimeWindowController>();
            _resolver.Resolve<TrainsTimetableController>();

            _resolver.Resolve<SemaphoresManager>().Init();
            _resolver.Resolve<RouteSemaphoresSupervisor>();
        }
    }
}
