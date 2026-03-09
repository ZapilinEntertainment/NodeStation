using System;
using UnityEngine;
using UniRx;

namespace ZE.NodeStation
{
    public interface IRouteHighlighlightController
    {
        IObservable<IRoute> ObservableHighlightedRoute { get; } 
        IRoute CurrentHighlightedRoute { get; }
    }
}
