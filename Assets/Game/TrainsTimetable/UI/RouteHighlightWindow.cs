using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

namespace ZE.NodeStation
{
    public class RouteHighlightWindow : UIWindowBase
    {
        public struct SelectedRouteData
        {
            public Color Color;
            public string RouteLabel;
            public IObservable<string> ObservableArrivalLabel;
            public IObservable<bool> ObservableIsRouteCorrect;
        }

        [SerializeField] private Image _colouredFrame;
        [SerializeField] private TextMeshProUGUI _routeLabel;
        [SerializeField] private TextMeshProUGUI _arrivalLabel;
        [SerializeField] private Button _cancelButton;
        private CompositeDisposable _compositeDisposable = new();
        private IDisposable _arrivalLabelSubscription;
        private const float FRAME_ALPHA = 0.5f;

        private void Awake()
        {
            WindowHideEvent += OnHide;
            DisposeEvent += OnDispose;
        }

        public void Setup(IObservable<SelectedRouteData> observableRouteData, Action cancelAction)
        {
            observableRouteData
                .Subscribe(OnRouteDataChanged)
                .AddTo(_compositeDisposable);

            _cancelButton
                .OnClickAsObservable()
                .Subscribe(_ => cancelAction.Invoke())
                .AddTo(_compositeDisposable);
        }

        private void OnRouteDataChanged(SelectedRouteData data)
        {
            _arrivalLabelSubscription?.Dispose();

            var color = data.Color; 
            color.a = FRAME_ALPHA;

            _colouredFrame.color = color;
            _routeLabel.text = data.RouteLabel;
            _arrivalLabelSubscription = data.ObservableArrivalLabel
                .Subscribe(text => _arrivalLabel.text = text);

            data.ObservableIsRouteCorrect
                .Subscribe(x => _colouredFrame.enabled = x)
                .AddTo(_compositeDisposable);
        }

        private void OnHide()
        {
            if (_arrivalLabelSubscription != null) 
            { 
                _arrivalLabelSubscription?.Dispose();
                _arrivalLabelSubscription = null;
            }

            _compositeDisposable.Clear();
        }

        private void OnDispose()
        {
            _compositeDisposable.Dispose();
            _arrivalLabelSubscription?.Dispose();   
        }
    }
}
