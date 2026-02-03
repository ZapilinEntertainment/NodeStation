using UnityEngine;

namespace ZE.NodeStation
{
    public interface IRailPath
    {
        PathKey PathKey { get; }
        RailPosition Start { get; }
        RailPosition End { get;}

        float Length { get; }

        RailPosition GetPosition(double distancePc, bool isReversed);
    }
}
