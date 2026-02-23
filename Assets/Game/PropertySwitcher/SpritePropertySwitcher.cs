using UnityEngine;

namespace ZE.NodeStation
{
    public class SpritePropertySwitcher : MonoPropertySwitcher
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite[] _sprites;

        protected override int StatesCount => _sprites.Length;

        protected override void OnStateChanged(int state)
        {
            _spriteRenderer.sprite = _sprites[state];
        }
    }
}
