using UnityEngine;
using UnityEngine.UI;

namespace ZE.NodeStation
{
    public class UISpritePropertySwitcher : MonoPropertySwitcher
    {
        [SerializeField] private Image _image;
        [SerializeField] private Sprite[] _sprites;

        protected override int StatesCount => _sprites.Length;

        protected override void OnStateChanged(int state)
        {
            _image.sprite = _sprites[state];
        }
    }
}
