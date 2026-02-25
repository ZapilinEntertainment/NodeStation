using System;
using UnityEngine;
using UniRx;

namespace ZE.NodeStation
{
    public class TimetabledTrain : IDisposable
    {
        public readonly TimeSpan LabelAppearTime;
        public readonly TimeSpan TrainLaunchTime;
        public readonly string LabelText;
        public readonly TimetabledTrainSpawnInfo SpawnInfo;
        public event Action DisposeEvent;

        public IReadOnlyReactiveProperty<TimetabledTrainStatus> StatusProperty => _statusProperty;
        public TimetabledTrainStatus Status { get => _statusProperty.Value; set => _statusProperty.Value = value; }
        public ITrain Train { get; private set;}    

        public bool IsReachedDestination => Train?.IsReachedDestination ?? false;
        public float MaxSpeed => SpawnInfo.TrainConfiguration.MaxSpeed;

        private ReactiveProperty<TimetabledTrainStatus> _statusProperty = new();


        public TimetabledTrain(TimeSpan labelAppearTime, TimeSpan launchTime, string labelText, in TimetabledTrainSpawnInfo spawnInfo)
        {
            LabelText = labelText;
            LabelAppearTime = labelAppearTime;
            TrainLaunchTime = launchTime;
            Status = TimetabledTrainStatus.NotReady;
            Train = null;
            SpawnInfo = spawnInfo;
        }

        public void OnTrainLaunched(ITrain train)
        {
            Train = train;
            _statusProperty.Value = TimetabledTrainStatus.Launched;
        }

        public void Dispose() 
        {
            if (Status == TimetabledTrainStatus.Disposed)
                return;

            Status = TimetabledTrainStatus.Disposed;
            DisposeEvent?.Invoke();
            _statusProperty.Dispose();
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
