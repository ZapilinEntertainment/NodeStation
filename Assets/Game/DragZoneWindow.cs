using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZE.NodeStation
{
    public class DragZoneWindow : MonoBehaviour, IBeginDragHandler, IDragHandler,  IEndDragHandler, IDragZone
    {
        public event Action DragStartEvent;
        public event Action DragEndEvent;

        public void OnBeginDrag(PointerEventData eventData) => DragEndEvent?.Invoke();

        public void OnDrag(PointerEventData eventData) { }

        public void OnEndDrag(PointerEventData eventData) => DragEndEvent?.Invoke();
    }
}
