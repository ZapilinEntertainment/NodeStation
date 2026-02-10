using System;
using UnityEngine;

namespace ZE.NodeStation
{
    public enum TimetabledTrainStatus : byte { NotReady, Announced, Launched, CompletedRoute, Disposed }
    public class TimetabledTrain : IDisposable
    {
        public readonly TimeSpan LabelAppearTime;
        public readonly TimeSpan TrainLaunchTime;
        public readonly string LabelText;
        public readonly TimetabledTrainSpawnInfo SpawnInfo;

        public TimetabledTrainStatus Status;
        public ITrain Train;
        public event Action DisposeEvent;

        public bool IsReachedDestination => Train.IsReachedDestination;

        public TimetabledTrain(TimeSpan labelAppearTime, TimeSpan launchTime, string labelText, in TimetabledTrainSpawnInfo spawnInfo)
        {
            LabelText = labelText;
            LabelAppearTime = labelAppearTime;
            TrainLaunchTime = launchTime;
            Status = TimetabledTrainStatus.NotReady;
            Train = null;
            SpawnInfo = spawnInfo;
        }

        public void Dispose() 
        {
            Status = TimetabledTrainStatus.Disposed;
            DisposeEvent?.Invoke();
        }
    }

    public struct TimetabledTrainSpawnInfo
    {
        public readonly RailPosition SpawnPosition;
        public readonly TrainConfiguration TrainConfiguration;

        public TimetabledTrainSpawnInfo(TrainConfiguration config, in RailPosition railPosition)
        {
            SpawnPosition = railPosition;
            TrainConfiguration = config;
        }
    }
}
