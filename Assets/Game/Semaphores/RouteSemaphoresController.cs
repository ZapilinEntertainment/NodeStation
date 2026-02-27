using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    // ignite semaphores before train (_igniteDistance), and distinguish after (_extinguishDistance)
    public class RouteSemaphoresController : IDisposable
    {
        public int ActiveSemaphoresCount { get; private set; } = 0;
        public float FirstBogieDist => _firstBogieDist;
        public float LastBogieDist => _lastBogieDist;

        private readonly IRoute _route;
        private readonly SemaphoresManager _semaphoresManager;
        private readonly IReadOnlyList<RouteSemaphoreData> _semaphoreData;
        private readonly float _igniteDistance;
        private readonly float _extinguishDistance;     
        private readonly BitArray _actualSemaphoresActivity;        
        
        private float _firstBogieDist;
        private float _lastBogieDist;

        public RouteSemaphoresController(
            IRoute route,
            SemaphoresManager semaphoresManager,
            IReadOnlyList<RouteSemaphoreData> semaphoresData,
            float igniteDistance, 
            float extinguishDistance,
            float firstBogieDistance, 
            float lastBogieDistance)
        {
            _route = route;
            _semaphoresManager = semaphoresManager;
            _semaphoreData = semaphoresData;
            _igniteDistance = igniteDistance;
            _extinguishDistance = extinguishDistance;
             
            _firstBogieDist = firstBogieDistance;
            _lastBogieDist = lastBogieDistance;

            _actualSemaphoresActivity = new(_semaphoreData.Count, false);    
        }

        public void OnTrainMove(float travelledDistance)
        {
            _firstBogieDist += travelledDistance;
            _lastBogieDist += travelledDistance;

            for (var i = 0; i < _semaphoreData.Count; i++)
            {
                var pos = _semaphoreData[i].Distance;

                var firstBogieDelta = _firstBogieDist - pos;
                if (firstBogieDelta < 0f && Mathf.Abs(firstBogieDelta) < _igniteDistance)
                {
                    SetSemaphoreActivity(i, true);
                    continue;
                }

                var lastBogieDelta = _lastBogieDist - pos;
                if (lastBogieDelta > _extinguishDistance)
                {
                    SetSemaphoreActivity(i, false);
                    continue;
                }
            }
        }

        public void Dispose()
        {
            for (var i = 0; i < _semaphoreData.Count; i++)
            {
                SetSemaphoreActivity(i, false);
            }
        }

        private void SetSemaphoreActivity(int index, bool isActive)
        {
            if (_actualSemaphoresActivity[index] == isActive)
                return;

            // first switch debug:
            if (isActive) 
            {
                //Debug.Log(_semaphoreData[index].Semaphore.MapPosition.Path);
                //var firstBogieDelta = _firstBogieDist - _semaphoreData[index].Distance;
                //Debug.Log(firstBogieDelta);
            }

            _actualSemaphoresActivity[index] = isActive;            
            var data = _semaphoreData[index];
            if (isActive)
                _semaphoresManager.IgniteSemaphore(data.Semaphore, _route, data.IsFront);
            else
                _semaphoresManager.DistinguishSemaphore(data.Semaphore, _route, data.IsFront);

            ActiveSemaphoresCount = 0;
            for (var i = 0; i < _actualSemaphoresActivity.Length; i++)
            {
                if (_actualSemaphoresActivity[i])
                    ActiveSemaphoresCount++;
            }
        }
    }
}
