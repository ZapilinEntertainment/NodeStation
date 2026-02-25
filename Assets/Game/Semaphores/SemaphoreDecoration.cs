using UnityEngine;

namespace ZE.NodeStation
{
    public class SemaphoreDecoration : MonoBehaviour
    {
        public struct SetupProtocol
        {
            public Color FrontColor;
            public Color RearColor;
        }

        [field: SerializeField] public MapPosition MapPosition { get; private set; }
        [SerializeField] private Light _frontLight;
        [SerializeField] private Light _rearLight;
        

        public void Setup(SetupProtocol protocol)
        {
            if (protocol.FrontColor != Color.clear)
            {
                _frontLight.color = protocol.FrontColor;
                _frontLight.enabled = true;
            }
            else
            {
                _frontLight.enabled = false;
            }

            if (protocol.RearColor != Color.clear)
            {
                _rearLight.color = protocol.RearColor;
                _rearLight.enabled = true;
            }
            else
            {
                _rearLight.enabled = false;
            }
        }
    }
}
