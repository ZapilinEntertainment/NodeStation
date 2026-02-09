using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public interface IDragZone
    {
        event Action DragStartEvent;
        event Action DragEndEvent;
    
    }
}
