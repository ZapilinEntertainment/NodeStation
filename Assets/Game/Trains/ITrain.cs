using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public interface ITrain : IViewable
    {
        void SetSpeed(float speedPc, bool isAccelerating);
        void Activate();
    }
}
