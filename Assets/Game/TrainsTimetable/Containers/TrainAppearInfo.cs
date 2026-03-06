using System;
using UnityEngine;

namespace ZE.NodeStation
{
    [Serializable]
    public struct TrainAppearInfo
    {
        public TimeStamp LabelAppearTime;
        public int WarningTimeInMinutes;
        public int SpawnDestinationIndex;
        public int TargetDestinationIndex;
        public TrainConfiguration TrainConfig;  
        public ColorKey ColorKey;

        public TimeSpan WarningTime => new (hours: 0, minutes: WarningTimeInMinutes, seconds: 0);
    }
}
