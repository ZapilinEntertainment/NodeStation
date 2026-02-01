using System;
using UnityEngine;

namespace ZE.NodeStation
{
    [Serializable]
    public struct RailPosition
    {
        public double Percent;
        public Vector3 WorldPosition;   
        public Quaternion WorldRotation;
    }
}
