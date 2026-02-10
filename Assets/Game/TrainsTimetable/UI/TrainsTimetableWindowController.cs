using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class TrainsTimetableWindowController : IDisposable
    {
        private readonly TrainsTimetableWindow _window;
        private readonly Dictionary<TimetabledTrain, TrainAppearLine> _lines = new();

        [Inject]
        public TrainsTimetableWindowController(TrainsTimetableWindow window)
        {
            _window = window;
        }

        public void AddLine(TimetabledTrain train)
        {

        }

        public void Dispose()
        {
            if (_lines.Count != 0)
            {
                foreach (var lineKvp in _lines)
                {
                    //lineKvp.Key.DisposeEvent -= RemoveLine;
                }
            }
        }

        private void RemoveLine(TimetabledTrain train)
        {

        }
    }
}
