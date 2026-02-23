using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace ZE.NodeStation
{
    public class ColliderSwitcher : MonoPropertySwitcher
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private bool[] _values;

        protected override int StatesCount => _values.Length;

        protected override void OnStateChanged(int state) => _collider.enabled = _values[state];
    }
}
