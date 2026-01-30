using UnityEngine;
using VContainer;

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
            
            // TODO: do rail positioning
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
                _position = _railMovementCalculator.MoveNext(_position, new(_speed * dt, _isReversed), _rail);
                transform.position = _position.WorldPosition;
            }
        }
    }
}
