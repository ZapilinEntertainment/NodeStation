using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class MultiBogeyTrainViewController : DisposableMonoBehaviour, IView, IHighlightable
    {
        private MultiBogeysTrain _train;
        private SceneViewsList _viewsList;
        private Dictionary<IHighlightable, Guid> _carsHighlightTickets = new();
        private Guid _activeHighlightTicket;

        public void Init(MultiBogeysTrain train, SceneViewsList viewsList)
        {
            _train = train;
            _train.OnViewSet(GetInstanceID());
            _viewsList = viewsList;
        }

        public Guid EnableHighlight()
        {
            foreach (var railCar in _train.Cars)
            {
                if (_viewsList.TryGetView(railCar.ViewId, out var view) && view is IHighlightable highlightable)
                {
                    _carsHighlightTickets.Add(highlightable, highlightable.EnableHighlight());
                }
            }
            _activeHighlightTicket = Guid.NewGuid();
            return _activeHighlightTicket;
        }

        public void DisableHighlight(Guid ticket)
        {
            if (ticket != _activeHighlightTicket)
                return;

            foreach (var kvp in _carsHighlightTickets)
            {
                if (kvp.Key == null)
                    continue;

                kvp.Key.DisableHighlight(kvp.Value);
            }
            _carsHighlightTickets.Clear();
            _activeHighlightTicket = default;
        }
    }
}
