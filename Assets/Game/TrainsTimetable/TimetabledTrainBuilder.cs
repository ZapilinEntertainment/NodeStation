using System;
using UnityEngine;
using VContainer;

namespace ZE.NodeStation
{
    public class TimetabledTrainBuilder
    {
        private readonly GetRouteStartPointCommand _getStartCommand;
        private readonly RouteBuilder _routeBuilder;
        private readonly TrainRoutesManager _routesManager;

        [Inject]
        public TimetabledTrainBuilder(GetRouteStartPointCommand getStartCommand, RouteBuilder routeBuilder, TrainRoutesManager routesManager)
        {
            _getStartCommand = getStartCommand;
            _routeBuilder = routeBuilder;
            _routesManager = routesManager;
        }

        public TimetabledTrain Build(in TrainAppearInfo trainAppearInfo)
        {
            var labelAppearTime = trainAppearInfo.LabelAppearTime.ToTimeSpan();
            var spawnPoint = _getStartCommand.Execute(trainAppearInfo.SpawnNodeKey);

            var train = new TimetabledTrain(
                labelAppearTime: labelAppearTime, 
                launchTime: labelAppearTime.Add(trainAppearInfo.WarningTime),
                labelText: BuildRouteLabel(trainAppearInfo),
                spawnInfo: new(trainAppearInfo.TrainConfig, spawnPoint));

            var routeParameters = new RouteSettings()
            {
                ColorKey = trainAppearInfo.ColorKey,
                SpawnNodeKey = trainAppearInfo.SpawnNodeKey,
                TargetNodeKey = trainAppearInfo.TargetNodeKey,
                IsReversed = spawnPoint.IsReversed
            };
            if (_routeBuilder.TryBuildRoute(routeParameters.SpawnNodeKey, routeParameters.ColorKey, out var trainRoute))
                _routesManager.SetRoute(train, trainRoute);

            return train;
        }

        private string BuildRouteLabel(in TrainAppearInfo trainAppearInfo)
        {
            // TODO: get names of nodes and combine
            return $"Node {trainAppearInfo.SpawnNodeKey} - Node {trainAppearInfo.TargetNodeKey}";
        }
    
    }
}
