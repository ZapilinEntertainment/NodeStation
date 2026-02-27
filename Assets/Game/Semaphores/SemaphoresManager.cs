using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class SemaphoresManager : IDisposable
    {
        public struct SemaphoreStatus
        {
            public IRoute FrontLightRoute;
            public IRoute RearLightRoute;
        }

        private readonly Dictionary<PathKey, SemaphoreDecoration> _semaphores = new();
        private readonly Dictionary<SemaphoreDecoration, SemaphoreStatus> _statuses = new();
        private readonly ILightColorsPalette _lightColors;

        [Inject]
        public SemaphoresManager(ILightColorsPalette lightColorsPalette)
        {
            _lightColors = lightColorsPalette;
        }

        public void Init()
        {
            var semaphores = GameObject.FindObjectsByType<SemaphoreDecoration>(FindObjectsSortMode.None);
            foreach (var semaphore in semaphores)
            {
                _semaphores.Add(semaphore.MapPosition.Path,semaphore);
            }
        }

        public void Dispose()
        {
            _semaphores.Clear();
            _statuses.Clear();
        }

        public bool TryGetSemaphore(PathKey pathKey, out SemaphoreDecoration semaphore) => _semaphores.TryGetValue(pathKey, out semaphore);

        public void IgniteSemaphore(SemaphoreDecoration semaphore, IRoute route, bool front)
        {
            if (_statuses.TryGetValue(semaphore, out var status))
            {
                if ((front && status.FrontLightRoute != null && status.FrontLightRoute != route) 
                    || (!front && status.RearLightRoute != null && status.RearLightRoute != route))
                {
                    Debug.LogWarning("routes conflict!");
                    // TODO: add visual notification
                }
            }
            
            if (front)
                status.FrontLightRoute = route;
            else
                status.RearLightRoute = route;

            UpdateSemaphoreStatus(semaphore, status);
        }

        public void DistinguishSemaphore(SemaphoreDecoration semaphore, IRoute route, bool front)
        {
            if (!_statuses.TryGetValue(semaphore, out var status))
                return;

            if (front && status.FrontLightRoute == route)
            {
                status.FrontLightRoute = null;
                UpdateSemaphoreStatus(semaphore, status);
                return;
            }           
            
            if (!front && status.RearLightRoute == route)
            {
                status.RearLightRoute = null;
                UpdateSemaphoreStatus(semaphore, status);
                return;
            }
        }
    
        private void UpdateSemaphoreStatus(SemaphoreDecoration semaphore, SemaphoreStatus status)
        {
            var frontRoute = status.FrontLightRoute;
            var frontColor = frontRoute == null ? Color.clear : _lightColors.GetLightColor(frontRoute.ColorKey);

            var rearRoute = status.RearLightRoute;
            var rearColor = rearRoute == null ? Color.clear : _lightColors.GetLightColor(rearRoute.ColorKey);

            semaphore.Setup(new() { FrontColor = frontColor, RearColor= rearColor });
            _statuses[semaphore] = status;
        }
    }
}
