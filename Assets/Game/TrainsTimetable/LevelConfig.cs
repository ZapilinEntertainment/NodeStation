using System;
using UnityEngine;

namespace ZE.NodeStation
{
    [CreateAssetMenu(fileName = nameof(LevelConfig), menuName = Constants.ScriptableObjectsFolderPath + nameof(LevelConfig))]
    public class LevelConfig : ScriptableObject
    {
        [field: SerializeField] public TimeStamp StartTime;
        [field: SerializeField] public DayOfWeek StartDayOfTheWeek;
        [field: SerializeField] public TimeStamp ShiftDuration;
        [field:SerializeField] public TrainAppearInfo[] Trains { get; set; }
    
    }
}
