using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public class SwitchableRoutePoint : MonoBehaviour, IColliderOwner, IDisposable
    {
        [SerializeField] private Collider _receivingCollider;
        public IPathNode Node { get;private set;}
        private bool _isDestroyed = false;

        public int GetColliderId() => _receivingCollider.GetInstanceID();

        public void AssignNode(IPathNode node)
        {
            Node = node;
        }

        public void Dispose()
        {
            if (_isDestroyed) return;
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            _isDestroyed = true;
        }
    }
}
