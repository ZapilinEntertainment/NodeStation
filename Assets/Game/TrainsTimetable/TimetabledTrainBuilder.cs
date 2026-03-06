using System;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class TimetabledTrainBuilder
    {
        private readonly GetRouteStartPointCommand _getStartCommand;
        private readonly RouteBuilder _routeBuilder;
        private readonly RoutesManager _routesManager;
        private readonly LevelConfig _levelConfig;

        [Inject]
        public TimetabledTrainBuilder(
            GetRouteStartPointCommand getStartCommand, 
            RouteBuilder routeBuilder, 
            RoutesManager routesManager,
            LevelConfig levelConfig)
        {
            _getStartCommand = getStartCommand;
            _routeBuilder = routeBuilder;
            _routesManager = routesManager;
            _levelConfig = levelConfig;
        }

        public TimetabledTrain Build(in TrainAppearInfo trainAppearInfo)
        {
            var labelAppearTime = trainAppearInfo.LabelAppearTime.ToTimeSpan();

            var spawnNodeKey = _levelConfig.Destinations[trainAppearInfo.SpawnDestinationIndex].NodeKey;
            var targetNodeKey = _levelConfig.Destinations[trainAppearInfo.TargetDestinationIndex].NodeKey;

            var spawnPoint = _getStartCommand.Execute(spawnNodeKey);

            var train = new TimetabledTrain(
                labelAppearTime: labelAppearTime, 
                launchTime: labelAppearTime.Add(trainAppearInfo.WarningTime),
                routeText: BuildRouteLabel(trainAppearInfo),
                spawnInfo: new(trainAppearInfo.TrainConfig, spawnPoint));

            var routeParameters = new RouteSettings()
            {
                ColorKey = trainAppearInfo.ColorKey,
                SpawnNodeKey = spawnNodeKey,
                TargetNodeKey = targetNodeKey,
                IsReversed = spawnPoint.IsReversed
            };
            if (_routeBuilder.TryBuildRoute(routeParameters.SpawnNodeKey, routeParameters.ColorKey, out var trainRoute))
                _routesManager.SetRoute(train, trainRoute);

            return train;
        }

        private string BuildRouteLabel(in TrainAppearInfo trainAppearInfo)
        {
            // TODO: get names of nodes and combine
            return $"{_levelConfig.Destinations[trainAppearInfo.SpawnDestinationIndex].NameKey} - {_levelConfig.Destinations[trainAppearInfo.TargetDestinationIndex].NameKey}";
        }
    
    }
}
