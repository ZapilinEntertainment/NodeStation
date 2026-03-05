using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    // TODO: rework to signal + controller
    public class SpawnTrainCommand
    {
        private readonly TrainFactory _trainFactory;

        public SpawnTrainCommand(TrainFactory trainFactory)
        {
            _trainFactory = trainFactory;
        }

        public ITrain Execute(TrainConfiguration config, in RailPosition position, float speedPercent, bool isAccelerating)
        {
            var train = _trainFactory.Build(config, position);
            train.SetSpeed(speedPercent, isAccelerating);
            return train;
        }

        public ITrain Execute(TimetabledTrain trainData)
        {
            var spawnInfo = trainData.SpawnInfo;
            var train = Execute(spawnInfo.TrainConfiguration, spawnInfo.SpawnPosition, 1f, true);
            return train;
        }
    }
}
