using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Pool;
using UniRx;

namespace ZE.NodeStation
{
    public class TrainTimetableLine : MonoBehaviour, IDisposable, IPoolable<TrainTimetableLine>
    {
        private enum LineState : byte { Disabled, AwaitingTrain, TrainArrived, Selected}

        public struct SetupProtocol
        {
            public Color Color;
            public string RouteLabel;
            public Action OnClickAction;
            public IReadOnlyReactiveProperty<TimetabledTrainStatus> StatusProperty;
            public IObservable<float> ArrivalProgressObservable;
            public IObservable<bool> IsLineSelectedObservable;
        }

        [SerializeField] private TextMeshProUGUI _routeLabel;
        [SerializeField] private MonoPropertyGroup _statusGroup;
        [SerializeField] private Image[] _colouringImages;
        [SerializeField] private Image _arrivalProgressImage;
        [SerializeField] private Button _button;

        private bool _isDestroyed = false;
        private bool _isLineSelected = false;
        private bool _isTrainArrived = false;
        private IObjectPool<TrainTimetableLine> _pool;
        private CompositeDisposable _subscriptions = new();
        private ReactiveCommand _buttonClickCommand = new();

        public void Setup(SetupProtocol protocol)
        {
            _routeLabel.text = protocol.RouteLabel;
            foreach (var image in _colouringImages)
            {
                image.color = protocol.Color;
            }
           
            _buttonClickCommand.Subscribe(_ => protocol.OnClickAction?.Invoke()).AddTo(_subscriptions);
            protocol.StatusProperty.Subscribe(OnStatusChanged).AddTo(_subscriptions);
            _buttonClickCommand.BindTo(_button).AddTo(_subscriptions);

            protocol.ArrivalProgressObservable
                .Subscribe(x => _arrivalProgressImage.fillAmount = x)
                .AddTo(_subscriptions);

            protocol.IsLineSelectedObservable
                .Subscribe(x => {_isLineSelected = x; UpdateState(); })
                .AddTo(_subscriptions);
        }

        public void SwitchToDisabled()
        {
            Clear();
            _statusGroup.SwitchState((int)LineState.Disabled);
        }

        public void Dispose() 
        {           
            if (_isDestroyed)
                return;
            _pool.Release(this);
        }

        public void AssignToPool(IObjectPool<TrainTimetableLine> pool) => _pool = pool;

        public void OnGet() 
        {
            gameObject.SetActive(true);
        }

        public void OnRelease() 
        {
            if (gameObject != null)
                gameObject.SetActive(false);
            Clear();
        }

        public void FinalDispose()
        {
            _subscriptions.Dispose();
            _pool = null;
            _buttonClickCommand.Dispose();
            Destroy(gameObject);            
        }

        private void OnStatusChanged(TimetabledTrainStatus status)
        {
            if (_isDestroyed) return;
            _button.interactable = status.CanChangeRoute();           
            _isTrainArrived = status == TimetabledTrainStatus.Launched;
            UpdateState();
        }

        private void Clear() 
        {
            _subscriptions.Clear();
            _isLineSelected = false;
            _isTrainArrived = false;
        }

        private void UpdateState()
        {
            if (_isLineSelected)
            {
                _statusGroup.SwitchState((int)LineState.Selected);
            }
            else
            {
                _statusGroup.SwitchState((int)(_isTrainArrived ? LineState.TrainArrived : LineState.AwaitingTrain));
            }
        }

        private void OnDestroy()
        {
            _isDestroyed = true;
        }
    }
}
