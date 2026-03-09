using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class WorldSpaceMarkersFactory
    {
        private readonly ICameraController _camera;
        private readonly WorldSpaceMarkerViewFactory _viewFactory;
        private readonly TickableManager _tickableManager;

        [Inject]
        public WorldSpaceMarkersFactory(ICameraController camera, WorldSpaceMarkerViewFactory viewFactory, TickableManager tickableManager)
        {
            _camera = camera;
            _viewFactory = viewFactory;
            _tickableManager = tickableManager;
        }

        public TrainDestinationMarker CreateTrainDestinationMarker(Vector3 worldPos)
        {
            var marker = new TrainDestinationMarker(                
                worldPos,
                _camera.WorldToScreen, 
                _viewFactory.CreateView(WorldSpaceMarkerViewKey.TrainDestination),
                _tickableManager);
            return marker;
        }
    }
}
