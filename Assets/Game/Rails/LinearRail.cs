using UnityEngine;

namespace ZE.NodeStation
{
    public class LinearRail : IPathSegment
    {
        private readonly PathKey _registrationKey;
        private readonly float _length;
        private readonly Vector3 _startPosition;
        private readonly Vector3 _endPosition;
        private readonly Quaternion _rotation;
        
        public float Length => _length;
        public PathKey PathKey => _registrationKey;

        public RailPosition Start => new() { Percent = 0f, WorldPosition = _startPosition, RawWorldRotation = _rotation, Rail = this };
        public RailPosition End => new() { Percent = 1f, WorldPosition = _endPosition, RawWorldRotation = _rotation, Rail = this };

        public LinearRail(in PathKey key, in Vector3 startPos, in Vector3 endPos)
        {
            _registrationKey = key;
            _startPosition = startPos;
            _endPosition = endPos;

            var dir = _endPosition - _startPosition;
            _length = dir.magnitude;
            _rotation = Quaternion.LookRotation(dir / _length, Vector3.up);
        }

        public RailPosition GetPosition(double distancePercent, bool isReversed) => new()
        {
            Percent = distancePercent,
            WorldPosition = Vector3.Lerp(_startPosition, _endPosition, (float)distancePercent),
            RawWorldRotation = _rotation,
            Rail = this,
            IsReversed = isReversed
        };
    }
}
