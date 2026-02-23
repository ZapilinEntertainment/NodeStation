using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace ZE.NodeStation
{
    public class MonoPropertyGroup : MonoPropertySwitcher
    {
        [SerializeField] private MonoPropertySwitcher[] _elements;
        protected override int StatesCount => _elements.Length;

        protected override void OnStateChanged(int state)
        {
            foreach (var element in _elements)
            {
                element.SwitchState(state);
            }
        }
    }
}
