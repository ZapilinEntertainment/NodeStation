using System;
using UnityEngine;
using UnityEngine.Pool;

namespace ZE.NodeStation
{
    public abstract class WorldSpriteDrawer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        protected bool IsDestroyed { get;private set; } = false;
        protected SpriteRenderer SpriteRenderer => _spriteRenderer;

        public void SetColor(Color color) => _spriteRenderer.color = color;
        public void SetPosition(Vector3 pos) => transform.position = pos;
        private void OnDestroy() => IsDestroyed = true;
    }
}
