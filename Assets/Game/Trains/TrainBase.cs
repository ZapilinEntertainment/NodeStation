using UnityEngine;
using VContainer;
using TriInspector;

namespace ZE.NodeStation
{
    public class TrainBase : MonoBehaviour
    {
        [SerializeField] private MapPosition _startPosition;
        [Space]

        [SerializeField] private float _maxSpeed = 1f;
        [SerializeField] private float _acceleration = 1f;
        [SerializeField] private float _deceleration = 0.5f;
        [SerializeField] private bool _isAccelerating = false;
        [SerializeField] private bool _isReversed = false;

        [Space]
        [DisplayAsString][SerializeField] private float DEBUG_railPercent;

        private float _speed = 0f;
        private RailPosition _position;
        private RailMovementCalculator _railMovementCalculator;
        private PathsMap _map;
        private IRailPath _rail;

        [Inject]
        public void Inject(RailMovementCalculator movementCalculator, PathsMap map)
        {
            _railMovementCalculator = movementCalculator;
            _map = map;
        }

        private void Start()
        {
            if (!_map.TryGetPath(_startPosition.Path, out var rail))
            {
                Debug.LogError("Train rail not exists");
                return;
            }          

            _rail = rail;
            _isReversed = _startPosition.IsReversed;
            SetPosition(_rail.GetPosition(_startPosition.Percent));
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            if (_isAccelerating)
            {
                _speed = Mathf.MoveTowards(_speed, _maxSpeed, dt * _acceleration);
            }
            else
            {
                _speed = Mathf.MoveTowards(_speed, 0f, dt * _deceleration);
            }

            if (_speed != 0f)
            {
                var movementResult = _railMovementCalculator.MoveNext(_position, new(_speed * dt, _isReversed), _rail);
                _rail = movementResult.Rail;
                SetPosition(movementResult.Position);
                if (movementResult.IsStopped)
                {
                    enabled = false;
                    return;
                }
            }
                
        }

        protected void SetPosition(in RailPosition pos)
        {
            transform.position = pos.WorldPosition;
            var rotation = pos.WorldRotation;
            if (_isReversed)
                rotation *= Quaternion.AngleAxis(180f, Vector3.up);
            transform.rotation = rotation;

            _position = pos;

            DEBUG_railPercent = (float)pos.Percent;
        }
    }
}
