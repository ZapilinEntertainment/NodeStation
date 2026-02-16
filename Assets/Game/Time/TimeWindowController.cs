using System;
using UnityEngine;
using VContainer;
using UniRx;

namespace ZE.NodeStation
{
    public class TimeWindowController : IDisposable
    {
        private readonly TimeManager _timeManager;
        private readonly TimeWindow _window;
        private readonly CompositeDisposable _compositeDisposable = new();

        [Inject]
        public TimeWindowController(TimeManager timeManager, TimeWindow window)
        {
            _timeManager = timeManager;
            _window = window;

            _timeManager.IsShiftEndedProperty
                .Select(x => !x)
                .Subscribe(_window.SetVisibility)
                .AddTo(_compositeDisposable);
            _timeManager.CurrentTimeProperty
                .Subscribe(DisplayTime)
                .AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }

        private void DisplayTime(TimeSpan span)
        {
            // TODO: add day of the week localized short string
            _window.Label.text = $"[{span.Days}] {span.Hours:D2}:{span.Minutes:D2}";
            _window.ProgressionBar.fillAmount = _timeManager.LevelProgress;
        }
    }
}
