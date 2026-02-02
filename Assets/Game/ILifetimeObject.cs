using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public interface ILifetimeObject
    {
        public event Action DisposedEvent;    
    }
}
