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
        public struct SetupProtocol
        {
            public string RouteLabel;
            public string TimeLabel;
            public Action OnClickAction;
            public IReadOnlyReactiveProperty<TimetabledTrainStatus> StatusProperty;
        }

        [SerializeField] private TextMeshProUGUI _routeLabel;
        [SerializeField] private TextMeshProUGUI _timeLabel;
        [SerializeField] private Image _statusImage;
        [SerializeField] private Button _button;
        private IObjectPool<TrainTimetableLine> _pool;
        private CompositeDisposable _subscriptions = new();
        private ReactiveCommand _buttonClickCommand = new();

        public void Setup(SetupProtocol protocol)
        {
            _routeLabel.text = protocol.RouteLabel;
            _timeLabel.text = protocol.TimeLabel;
           
            _buttonClickCommand.Subscribe(_ => protocol.OnClickAction?.Invoke()).AddTo(_subscriptions);
            protocol.StatusProperty.Subscribe(OnStatusChanged).AddTo(_subscriptions);
            _buttonClickCommand.BindTo(_button).AddTo(_subscriptions);
        }

        public void Dispose() => _pool.Release(this);

        public void AssignToPool(IObjectPool<TrainTimetableLine> pool) => _pool = pool;

        public void OnGet() { }

        public void OnRelease() 
        {
            _subscriptions.Dispose();
            _subscriptions = null;
        }

        public void FinalDispose()
        {
            _pool = null;
            _buttonClickCommand.Dispose();
            Destroy(gameObject);
        }

        private void OnStatusChanged(TimetabledTrainStatus status)
        {
            _button.interactable = status.CanChangeRoute();
        }
    }
}
