using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public interface ILineDrawer : IDisposable
    {
        void DrawPoints(Vector3[] points);
    
    }
}
