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
            var sprite = _sprites[state];
            _image.sprite = sprite;
            _image.enabled = sprite == null ? false : true;            
        }
    }
}
