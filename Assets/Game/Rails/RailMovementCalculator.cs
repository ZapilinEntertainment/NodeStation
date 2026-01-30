using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class RailMovementCalculator
    {
        private readonly PathsMap _map;

        [Inject]
        public RailMovementCalculator(PathsMap map)
        {
            _map = map;
        }

        public RailPosition MoveNext(in RailPosition startPos, in RailMovement movement, IRailPath rail)
        {
            // todo: return train crash result

            var railLength = rail.Length;
            var percentMovement = movement.Distance / railLength;
            var reverseMovement = movement.IsReversed;
            if (reverseMovement)
            {
                // from end to start
                if (startPos.Percent >= percentMovement)
                    return rail.GetPosition(startPos.Percent - percentMovement);
            }
            else
            {
                // from start to end
                var nextPc = startPos.Percent + percentMovement;
                if (nextPc < 1f)
                    return rail.GetPosition(nextPc);                
            }
            
            if (!_map.TryGetNextRail(rail, out var nextRail, reverseMovement))
            {
                // train crash:
                return reverseMovement ? rail.Start : rail.End;
            }            

            var distanceLeft = (float)((percentMovement - 1f) * railLength);
            var startPoint = reverseMovement ? nextRail.End : nextRail.Start;
            return MoveNext(startPoint, new (distanceLeft, reverseMovement), nextRail);
        }
    
    }
}
