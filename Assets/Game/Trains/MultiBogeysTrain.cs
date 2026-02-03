using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    public class MultiBogeysTrain : TrainBase
    {
        private RailCar[] _cars;
        private float[] _distances;

        public MultiBogeysTrain(
            InjectProtocol protocol, 
            TrainConfiguration config, 
            ILifetimeObject lifetimeObject) : base(protocol, config, lifetimeObject)
        {
            DisposedEvent += DisposeCars;
        }

        public void SetupTrain(params RailCar[] cars)
        {
            _cars = cars;
            var count = _cars.Length;
            _distances = new float[count-1];

            for (var i = 0; i < count-1; i++)
            {
                _distances[i] = (_cars[i].CarLength + _cars[i + 1].CarLength) * 0.5f;
            }
        }

        public override void SetPosition(in RailPosition pos)
        {
            base.SetPosition(pos);

            var bogeyPos = _position;
            for (var i = 0; i < _cars.Length; i++)
            {
                var car = _cars[i];
                // note: reversed !_isReversed argument (for rear bogey position calculation)
                var movement = new RailMovement(car.BogeysDistance, !_isReversed);
                var rearBogeyPos = RailMovementCalculator.MoveNext(bogeyPos, movement).Position;
                rearBogeyPos.IsReversed = _isReversed;

                car.SetPosition(bogeyPos, rearBogeyPos);
                if (i != _cars.Length - 1)
                {

                    // note: some proble with distances here (only between passenger car and locomotive)
                    var nextPos = RailMovementCalculator.MoveNext(bogeyPos, new(_distances[i], !_isReversed)).Position;
                    nextPos.IsReversed = _isReversed;
                    bogeyPos = nextPos;
                }
                    
            }
        }

        private void DisposeCars()
        {
            DisposedEvent -= DisposeCars;

            var carsCount = _cars.Length;
            if (carsCount == 0)
                return;

            for (var i = 0; i < carsCount; i++)
            {
                _cars[i].Dispose();
            }
        }
    }
}
