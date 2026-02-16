using System;
using UnityEngine;

namespace ZE.NodeStation
{
    [Serializable]
    public struct TrainAppearInfo
    {
        public TimeStamp LabelAppearTime;
        public int WarningTimeInMinutes;
        public int SpawnNodeKey;
        public int TargetNodeKey;
        public TrainConfiguration TrainConfig;  
        public ColorKey ColorKey;

        public TimeSpan WarningTime => new (hours: 0, minutes: WarningTimeInMinutes, seconds: 0);
    }
}
