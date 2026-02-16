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
        
        private TrainRoute _currentRoute;

        [InfoBox("Available only in Playmode")]
        [Button("Draw route"), EnableInPlayMode]
        public void DrawRoute()
        {
            if (_currentRoute == null) 
                PrepareRouteWithExistingParameters();
            _routeDrawManager.DrawRoute(_currentRoute);
        }

        protected override RailPosition GetSpawnPosition()
        {
            if (_currentRoute == null) 
                PrepareRouteWithExistingParameters();
            return _getRouteStartPointCommand.Execute(_currentRoute, _routeTargets.IsReversed ? 0f : 1f, _routeTargets.IsReversed);
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
            _currentRoute = route;
        }

        private void ClearCurrentRoute()
        {
            if (_currentRoute == null)
                return;

            _routeDrawManager.ClearRouteDrawing(_currentRoute);
        }

        private void ApplyNewRoute(TrainRoute route)
        {
            ClearCurrentRoute();
            _routeDrawManager.DrawRoute(_currentRoute);
        }
    }
}
