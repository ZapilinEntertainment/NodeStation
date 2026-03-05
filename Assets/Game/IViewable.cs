using UnityEngine;

namespace ZE.NodeStation
{
    // can have mono-view
    public interface IViewable : ILifetimeObject
    {
        int ViewId { get;}

        Vector3 WorldPosition { get; }
        Quaternion WorldRotation { get; }

        void OnViewSet(int viewKey);

    }
}
