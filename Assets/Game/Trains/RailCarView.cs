using System;
using UnityEngine;

namespace ZE.NodeStation
{
    // todo: make poolable
    public class RailCarView : MonoView<RailCar>, IHighlightable
    {
        [SerializeField] private GameObject[] _highlightableObjects;
        [SerializeField] private GameObject[] _ignoreGrayscaleObjects;
        [SerializeField] private GameObject[] _disableOnHighlightObjects;
        private bool _defaultLayersWasSaved = false;
        private int[] _savedLayers;
        private Guid _activeHighlightTicket;

        public Guid EnableHighlight()
        {
            if (IsDisposed)
                return default; 

            if (!_defaultLayersWasSaved)
                SaveDefaultLayers();

            foreach (var obj in _highlightableObjects)
            {
                obj.layer = Constants.HIGHLIGHT_COLOURED_LAYER;
            }

            foreach (var obj in _ignoreGrayscaleObjects)
            {
                obj.layer = Constants.GRAYSCALE_IGNORE_LAYER;
            }

            foreach (var obj in _disableOnHighlightObjects)
            {
                obj.SetActive(false);
            }

            _activeHighlightTicket = Guid.NewGuid();
            return _activeHighlightTicket;
        }

        public void DisableHighlight(Guid ticket)
        {
            if (IsDisposed ||  _activeHighlightTicket != ticket)
                return;

            
            for (var i = 0; i < _highlightableObjects.Length; i++)
            {
                _highlightableObjects[i].layer = _savedLayers[i];
            }

            var delta = _highlightableObjects.Length;
            for (var i = 0; i < _ignoreGrayscaleObjects.Length; i++)
            {
                _ignoreGrayscaleObjects[i].layer = _savedLayers[i + delta];
            }

            foreach (var obj in _disableOnHighlightObjects)
            {
                obj.SetActive(true);
            }

            _activeHighlightTicket = default;
        }

        private void SaveDefaultLayers()
        {
            _savedLayers = new int[_highlightableObjects.Length + _ignoreGrayscaleObjects.Length];

            for (var i = 0; i < _highlightableObjects.Length; i++)
            {
                _savedLayers[i] = _highlightableObjects[i].layer;
            }

            var delta = _highlightableObjects.Length;
            for (var i = 0; i < _ignoreGrayscaleObjects.Length; i++)
            {
                _savedLayers[i + delta] = _ignoreGrayscaleObjects[i].layer;
            }

            _defaultLayersWasSaved = true;
        }
    }
}
