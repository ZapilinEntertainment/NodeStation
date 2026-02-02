using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public interface ITrain : ILifetimeObject
    {
        Vector3 WorldPosition { get; }
        Quaternion WorldRotation { get; }

        void SetSpeed(float speedPc, bool isAccelerating);
        void Activate();
    }
}
