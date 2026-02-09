using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public interface IPointDrawer : IDisposable
    {
        void SetPosition(Vector3 position);
    
    }
}
