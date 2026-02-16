using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ZE.NodeStation
{
    public class TimeWindow : MonoBehaviour
    {
        [field:SerializeField] public TextMeshProUGUI Label { get;private set;}
        [field: SerializeField] public Image ProgressionBar { get; private set;}

        public void SetVisibility(bool x) => gameObject.SetActive(x);
    }
}
