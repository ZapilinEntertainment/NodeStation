using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public interface IHighlightable
    {
        Guid EnableHighlight();
        void DisableHighlight(Guid ticket);
    }
}
