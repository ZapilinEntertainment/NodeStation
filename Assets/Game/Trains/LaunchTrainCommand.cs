using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    // TODO: rework to signal + controller
    public class LaunchTrainCommand
    {

        private readonly TrainFactory _trainFactory;

        public LaunchTrainCommand(TrainFactory trainFactory)
        {
            _trainFactory = trainFactory;
        }

        public void Execute(TrainConfiguration config, in RailPosition position, float speedPercent, bool isAccelerating)
        {
            var train = _trainFactory.Build(config, position);
            train.SetSpeed(speedPercent, isAccelerating);
        }

        public void Execute(TimetabledTrain trainData)
        {
            var spawnInfo = trainData.SpawnInfo;
            var train = _trainFactory.Build(spawnInfo.TrainConfiguration, spawnInfo.SpawnPosition); 
            train.SetSpeed(1f, true);
        }
    }
}
