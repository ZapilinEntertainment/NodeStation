using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZE.NodeStation
{
    public class DragZoneWindow : MonoBehaviour, IBeginDragHandler, IDragHandler,  IEndDragHandler, IDragZone
    {
        public Vector2 DragScreenPos { get; private set; }
        public event Action DragStartEvent;
        public event Action DragEndEvent;

        public void OnBeginDrag(PointerEventData eventData) => DragStartEvent?.Invoke();

        public virtual void OnDrag(PointerEventData eventData) => DragScreenPos = eventData.position;

        public void OnEndDrag(PointerEventData eventData) => DragEndEvent?.Invoke();
    }
}
