using UnityEngine;

namespace ZE.NodeStation
{
    // can have mono-view
    public interface IViewable : ILifetimeObject
    {
        public Vector3 WorldPosition { get; }
        public Quaternion WorldRotation { get; }

    }
}
