using UnityEngine;
using VContainer;
using TriInspector;

namespace ZE.NodeStation
{
    public class EditorRouteLauncher : EditorTrainLauncherBase
    {
        [SerializeField] private RouteSettings _routeTargets;
        [Inject] private RouteBuilder _routeBuilder;
        [Inject] private GetRouteStartPointCommand _getRouteStartPointCommand;
        [Inject] private RouteDrawManager _routeDrawManager;
        
        private RouteController _activeRouteController;
        private IRoute CurrentRoute => _activeRouteController.Route;

        [InfoBox("Available only in Playmode")]
        [Button("Draw route"), EnableInPlayMode]
        public void DrawRoute()
        {
            if (_activeRouteController == null) 
                PrepareRouteWithExistingParameters();
            _routeDrawManager.DrawRoute(_activeRouteController.Route);
        }

        protected override RailPosition GetSpawnPosition()
        {
            if (_activeRouteController == null) 
                PrepareRouteWithExistingParameters();
            return _getRouteStartPointCommand.Execute(CurrentRoute, _routeTargets.IsReversed ? 0f : 1f, _routeTargets.IsReversed);
        }
        
        private void PrepareRouteWithExistingParameters()
        {
            if (!_map.TryGetNode(_routeTargets.SpawnNodeKey, out var spawnNode))
            {
                Debug.LogError("invalid route start node");
                return;
            }

            if (!_routeBuilder.TryBuildRoute(_routeTargets.SpawnNodeKey, _routeTargets.ColorKey, out var route))
            {
                Debug.LogError("route build failed");
                return;
            }
            ClearCurrentRoute();
            _activeRouteController = route;
        }

        private void ClearCurrentRoute()
        {
            if (_activeRouteController == null)
                return;

            _routeDrawManager.ClearRouteDrawing(CurrentRoute);
        }

        private void ApplyNewRoute(TrainRoute route)
        {
            ClearCurrentRoute();
            _routeDrawManager.DrawRoute(CurrentRoute);
        }
    }
}
