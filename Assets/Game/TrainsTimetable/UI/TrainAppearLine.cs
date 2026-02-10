using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ZE.NodeStation
{
    public class TrainAppearLine : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _routeLabel;
        [SerializeField] private TextMeshProUGUI _timeLabel;
        [SerializeField] private Image _timerImage;
    }
}
