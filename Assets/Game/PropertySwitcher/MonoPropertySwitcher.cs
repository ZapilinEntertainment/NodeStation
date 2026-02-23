using UnityEngine;
using TriInspector;

namespace ZE.NodeStation
{
    public abstract class MonoPropertySwitcher : MonoBehaviour
    {
        [SerializeField, OnValueChanged(nameof(EDITOR_ChangeState))] private int _currentState;


        protected abstract int StatesCount { get; }

        public void SwitchState(int state)
        {
            state = Mathf.Clamp(state, 0, StatesCount);
            _currentState = state;
            OnStateChanged(state);
        }

        protected abstract void OnStateChanged(int state);
        private void EDITOR_ChangeState() 
        {
            SwitchState(_currentState);
            Debug.Log(_currentState);
        }

#if UNITY_EDITOR
        [Button(ButtonSizes.Small, "-")]
        [Group("Buttons")]
        private void EDITOR_DecreaseState() => SwitchState(_currentState - 1);

        [Button(ButtonSizes.Small, "+")]
        [Group("Buttons")]
        private void EDITOR_IncreaseState() => SwitchState(_currentState + 1);
#endif

    }
}
