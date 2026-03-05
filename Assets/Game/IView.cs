using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public interface IView : IDisposable
    {
        int GetInstanceID();
        event Action DisposeEvent;
    }
}
