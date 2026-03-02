using UnityEngine;

namespace ZE.NodeStation
{
    public class ActivityPropertySwitcher : MonoPropertySwitcher
    {
        [SerializeField] private bool[] _values;

        protected override int StatesCount => _values.Length;

        protected override void OnStateChanged(int state) => gameObject.SetActive(_values[state]);

    }
}
