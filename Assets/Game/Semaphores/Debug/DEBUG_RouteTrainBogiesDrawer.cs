using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    #if UNITY_EDITOR
    public class DEBUG_RouteTrainBogiesDrawer : MonoBehaviour
    {
        [SerializeField] private Color _frontColor = Color.blue;
        [SerializeField] private Color _rearColor = Color.red;
        [SerializeField] private float _frontOffset = 0f;
        [SerializeField] private float _rearOffset = 0f;

        private RailMovementCalculator _calculator;        
        private PathsMap _pathsMap;
        private RouteSemaphoresController _controller;
        private IRoute _route;

        [Inject]
        public void Inject(RailMovementCalculator calculator, PathsMap map)
        {
            _calculator = calculator;
            _pathsMap = map;
        }

        public void SetupRoute(IRoute route, RouteSemaphoresController controller)
        {
            _route = route;
            _controller = controller;      
        }

        private void OnDrawGizmos()
        {
            if (_controller == null || _route == null)
                return;

            var lastBogieDist = _controller.LastBogieDist + _rearOffset;
            if (lastBogieDist < 0f) lastBogieDist = 0f;
            var lastBogieDistDrawn = false;

            var firstBogieDist = _controller.FirstBogieDist + _frontOffset;
            if (firstBogieDist < 0f) firstBogieDist = 0f;
            var firstBogieDistDrawn = false;

            var dist = 0f;

            for (var i = 1; i < _route.Points.Count; i++)
            {
                var pathKey = new PathKey (_route.Points[i].Key, _route.Points[i-1].Key);
                _pathsMap.TryGetPath(pathKey, out var path);
                var newDist = dist + path.Length;

                if (!lastBogieDistDrawn && lastBogieDist < newDist)
                {
                    var pc = (lastBogieDist - dist) / path.Length;
                    var worldPosA = _route.Points[i].WorldPosition;
                    var worldPosB = _route.Points[i-1].WorldPosition;

                    Gizmos.color = _rearColor;
                    Gizmos.DrawSphere(Vector3.Lerp(worldPosB, worldPosA, pc), 1f);
                    lastBogieDistDrawn = true;
                }

                if (!firstBogieDistDrawn && firstBogieDist < newDist)
                {
                    var pc = (firstBogieDist - dist) / path.Length;
                    var worldPosA = _route.Points[i].WorldPosition;
                    var worldPosB = _route.Points[i - 1].WorldPosition;

                    Gizmos.color = _frontColor;
                    Gizmos.DrawSphere(Vector3.Lerp(worldPosB, worldPosA, pc), 1f);
                    firstBogieDistDrawn = true;
                }

                dist = newDist;

                // TODO: there is a big problem in spawning trains, need new exact logic!
            }
        }
    
    }
    #endif
}
