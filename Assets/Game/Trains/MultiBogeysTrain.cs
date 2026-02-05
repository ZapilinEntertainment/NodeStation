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
                var frontCar = _cars[i];
                var rearCar = _cars[i+1];
                // note: rear bogie offset is negative
                _distances[i] = 0.5f * frontCar.CarLength + frontCar.RearBogie.Offset + 0.5f * rearCar.CarLength - rearCar.FrontBogie.Offset;
            }
        }

        public override void SetPosition(in RailPosition pos)
        {
            base.SetPosition(pos);

            var frontBogiePos = _position;
            for (var i = 0; i < _cars.Length; i++)
            {
                var car = _cars[i];
                // note: reversed !_isReversed argument (for rear bogey position calculation)
                var movement = new RailMovement(car.BogeysDistance, !_isReversed);
                var rearBogiePos = RailMovementCalculator.MoveNext(frontBogiePos, movement).Position;
                rearBogiePos.IsReversed = _isReversed;

                car.SetPosition(frontBogiePos, rearBogiePos);
                if (i != _cars.Length - 1)
                {
                    var nextPos = RailMovementCalculator.MoveNext(rearBogiePos, new(_distances[i], !_isReversed)).Position;
                    nextPos.IsReversed = _isReversed;
                    frontBogiePos = nextPos;
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
