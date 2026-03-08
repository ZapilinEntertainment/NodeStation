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
        private readonly PathsMap _map;

        [Inject]
        public TimetabledTrainBuilder(
            GetRouteStartPointCommand getStartCommand, 
            RouteBuilder routeBuilder, 
            RoutesManager routesManager,
            LevelConfig levelConfig,
            PathsMap map)
        {
            _getStartCommand = getStartCommand;
            _routeBuilder = routeBuilder;
            _routesManager = routesManager;
            _levelConfig = levelConfig;
            _map = map;
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

            if (_routeBuilder.TryBuildRoute(spawnNodeKey, trainAppearInfo.ColorKey, out var trainRoute))
            {
                _routesManager.SetRoute(train, trainRoute);
                if (_map.TryGetNode(targetNodeKey, out var targetNode))
                    trainRoute.SetTargetNode(targetNode);
                else
                    Debug.LogError("Invalid target node!");
            }
                

            return train;
        }

        private string BuildRouteLabel(in TrainAppearInfo trainAppearInfo)
        {
            // TODO: get names of nodes and combine
            return $"{_levelConfig.Destinations[trainAppearInfo.SpawnDestinationIndex].NameKey} - {_levelConfig.Destinations[trainAppearInfo.TargetDestinationIndex].NameKey}";
        }
    
    }
}
