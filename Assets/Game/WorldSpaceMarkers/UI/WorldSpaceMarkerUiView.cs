using UnityEngine;

namespace ZE.NodeStation
{
    public class WorldSpaceMarkerUiView : DisposableMonoBehaviour, IWorldMarkerUiView
    {
        [SerializeField] protected RectTransform RectTransform;

        public virtual void SetPosition(Vector2 screenPos) 
        {
            if (IsDisposed)
                return;
            transform.position = screenPos;
        }

        public void SetVisible(bool visible) => gameObject.SetActive(visible);
    }
}
