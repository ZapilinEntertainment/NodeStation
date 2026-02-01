using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public struct MovementResult
    {
        public RailPosition Position;
        public IRailPath Rail;
        public bool IsStopped;

        public MovementResult(RailPosition position, IRailPath rail)
        {
            Position = position;
            Rail = rail;
            IsStopped = false;
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

        public MovementResult MoveNext(in RailPosition startPos, in RailMovement movement, IRailPath rail)
        {
            // todo: return train crash result

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
                // train crash:
                Debug.Log("next rail not found!");
                return new() { Position = reverseMovement ? rail.Start : rail.End, Rail = rail, IsStopped = true };
            }        
            //Debug.Log($"rail changed: {rail.RegistrationKey} -> {nextRail.RegistrationKey}, {resultingPercent}");

            var distanceLeft = (float)((reverseMovement? (1f -resultingPercent) : resultingPercent) * railLength);

            var startPoint = reverseMovement ? nextRail.End : nextRail.Start;   
            return MoveNext(startPoint, new (distanceLeft, reverseMovement), nextRail);
        }
    
    }
}
