using System;
using UnityEngine;
using UnityEngine.UI;

namespace ZE.NodeStation
{
    public class ButtonActivityPropertySwitcher : MonoPropertySwitcher
    {
        [Serializable]
        public struct ButtonActivity
        {
            public bool IsVisible;
            public bool IsInteractable;
        }

        [SerializeField] private Button _button;
        [SerializeField] private ButtonActivity[] _activity;
        protected override int StatesCount => _activity.Length;

        protected override void OnStateChanged(int state)
        {
            var activity = _activity[state];
            _button.interactable = activity.IsInteractable;
            _button.enabled = activity.IsVisible;
        }
    }
}
