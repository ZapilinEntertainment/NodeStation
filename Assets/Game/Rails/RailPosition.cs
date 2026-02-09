using System;
using UnityEngine;

namespace ZE.NodeStation
{
    [Serializable]
    public struct RailPosition
    {
        public IPathSegment Rail;
        public double Percent;
        public bool IsReversed;
        public Vector3 WorldPosition;   
        public Quaternion RawWorldRotation;

        public Quaternion WorldRotation => IsReversed ? Quaternion.AngleAxis(180f, Vector3.up) * RawWorldRotation : RawWorldRotation;
    }
}
