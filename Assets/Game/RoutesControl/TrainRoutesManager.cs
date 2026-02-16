using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    public class TrainRoutesManager : IDisposable
    {
        private readonly Dictionary<TimetabledTrain, TrainRoute> _routes = new();
    

        public void SetRoute(TimetabledTrain train, TrainRoute route)
        {
            _routes.Add(train, route);
            train.DisposeEvent += () => this.ClearRoute(train);
        }

        public void ClearRoute(TimetabledTrain train)
        {
            if (_routes.TryGetValue(train, out var route))
            {
                route.Dispose();
                _routes.Remove(train);
            }
        }

        public bool TryGetRoute(TimetabledTrain train, out TrainRoute route) => _routes.TryGetValue(train, out route);

        public void Dispose()
        {
            foreach (var route in _routes.Values)
            {
                route?.Dispose();
            }
            _routes.Clear();
        }
    }
}
