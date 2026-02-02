using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public enum PostMovementEventType : byte { None, Derail, Disappear}

    public struct MovementResult
    {
        public RailPosition Position;
        public IRailPath Rail;
        public PostMovementEventType EventType;

        public MovementResult(RailPosition position, IRailPath rail, PostMovementEventType eventType = PostMovementEventType.None)
        {
            Position = position;
            Rail = rail;
            EventType = eventType;
        }
    }

    public class RailMovementCalculator
    {
        private readonly PathsMap _map;

        [Inject]
        public RailMovementCalculator(PathsMap map)
        {
            _map = map;
        }

        public MovementResult MoveNext(in RailPosition startPos, in RailMovement movement)
        {
            // todo: return train crash result

            var rail = startPos.Rail;
            var railLength = rail.Length;
            var percentMovement = movement.Distance / railLength;
            var reverseMovement = movement.IsReversed;
            double resultingPercent;

            if (reverseMovement)
            {
                // from end to start
                resultingPercent = startPos.Percent - percentMovement;

                if (resultingPercent > 0f)
                    return new(rail.GetPosition(resultingPercent), rail);
                else
                    resultingPercent += 1f;
            }
            else
            {
                // from start to end
                resultingPercent = startPos.Percent + percentMovement;

                if (resultingPercent < 1f)
                    return new(rail.GetPosition(resultingPercent), rail);    
                else
                    resultingPercent -= 1f;
            }            

            if (!_map.TryGetNextRail(rail, out var nextRail, reverseMovement))
            {
                var pathKey = rail.PathKey;
                var endPoint = reverseMovement ? pathKey.StartNodeKey : pathKey.EndNodeKey;
                var stopMode = _map.IsFinalNode(endPoint) ? PostMovementEventType.Disappear : PostMovementEventType.Derail;
                return new(reverseMovement ? rail.Start : rail.End, rail, stopMode);
            }        
            //Debug.Log($"rail changed: {rail.RegistrationKey} -> {nextRail.RegistrationKey}, {resultingPercent}");

            var distanceLeft = (float)((reverseMovement? (1f -resultingPercent) : resultingPercent) * railLength);

            var startPoint = reverseMovement ? nextRail.End : nextRail.Start;   
            return MoveNext(startPoint, new (distanceLeft, reverseMovement));
        }
    
    }
}
