using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public interface ITrain : IViewable
    {
        bool IsReachedDestination { get;}
        void SetSpeed(float speedPc, bool isAccelerating);
        void Activate();
    }
}
