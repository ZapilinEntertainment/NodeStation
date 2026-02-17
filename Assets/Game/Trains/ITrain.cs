using System;
using UnityEngine;
using UniRx;

namespace ZE.NodeStation
{
    public interface ITrain : IViewable
    {
        bool IsReachedDestination { get;}
        void SetSpeed(float speedPc, bool isAccelerating);
        void Activate();

        RailPosition FirstBogiePosition { get;}
        IReadOnlyReactiveProperty<RailPosition> RailPositionProperty { get; }
    }
}
