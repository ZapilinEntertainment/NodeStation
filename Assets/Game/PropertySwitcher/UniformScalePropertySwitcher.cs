using UnityEngine;

namespace ZE.NodeStation
{
    public class UniformScalePropertySwitcher : MonoPropertySwitcher
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private float[] _scales;

        protected override int StatesCount => _scales.Length;

        protected override void OnStateChanged(int state)
        {
            _transform.localScale = _scales[state] * Vector3.one;
        }
    }
}
