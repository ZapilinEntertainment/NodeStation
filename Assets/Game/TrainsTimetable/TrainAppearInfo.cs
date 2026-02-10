using System;
using UnityEngine;

namespace ZE.NodeStation
{
    [Serializable]
    public struct TrainAppearInfo
    {
        public TimeStamp LabelAppearTime;
        public TimeStamp TrainAppearTime;
        public int SpawnNodeKey;
        public int TargetNodeKey;
        public TrainConfiguration TrainConfig;    
    }
}
