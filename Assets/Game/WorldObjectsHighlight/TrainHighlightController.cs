using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;

namespace ZE.NodeStation
{
    // highlights train if their routes are selected (and train exists)
    public class TrainHighlightController : IDisposable, IStartable
    {
        private readonly ISceneFlagsManager _flags;
        private readonly ITimetabledTrainsList _trainsList;
        private readonly SceneViewsList _viewsList;
        private readonly CompositeDisposable _compositeDisposable = new();

        private Guid _highlightedTrainTicket;
        private IHighlightable _highlightedTrain;
        private IDisposable _trainStatusSubscription;

        [Inject]
        public TrainHighlightController(
            ISceneFlagsManager flags, 
            ITimetabledTrainsList trainsList,
            SceneViewsList viewsList)
        {
            _flags = flags;
            _trainsList = trainsList;
            _viewsList = viewsList;
        }

        public void Start()
        {
            _flags
                .Subscribe<TrainRouteHighlightFlag>(OnTrainHighlightFlagChanged)
                .AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            StopHighlight();
            _compositeDisposable.Dispose();
        }

        private void OnTrainHighlightFlagChanged(bool isActive)
        {
            if (isActive)
            {
                var timetabledTrain = _flags.GetFirstFlag<TrainRouteHighlightFlag>().Train;

                _trainStatusSubscription = timetabledTrain.StatusProperty
                    .Where(status => status == TimetabledTrainStatus.Launched)
                    .Take(1)
                    .Subscribe(_ => HighlightTrain(timetabledTrain.Train));
            }
            else
            {
                StopHighlight();
            }                
        }

        private void HighlightTrain(ITrain train)
        {            
            if (_viewsList.TryGetView(train.ViewId, out var view) && view is IHighlightable highlightable)
            {
                _highlightedTrain = highlightable;
                _highlightedTrainTicket = highlightable.EnableHighlight();
            }
            
        }

        private void StopHighlight()
        {
            if (_trainStatusSubscription == null)
                return;

            _highlightedTrain?.DisableHighlight(_highlightedTrainTicket);
            _highlightedTrainTicket = default;

            _trainStatusSubscription.Dispose();
            _trainStatusSubscription = null;
        }
    }
}
