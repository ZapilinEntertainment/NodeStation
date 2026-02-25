using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    public class MultiBogeysTrain : TrainBase
    {
        private RailCar[] _cars;
        private float[] _distances;
        private RailPosition _lastBogiePosition;

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

        protected override void DoMove(float deltaTime)
        {
            var movement = new RailMovement(Speed * deltaTime, _isReversed);
            var carsCount = _cars.Length;

            // handle locomotive movement
            var locomotive = _cars[0];
            var front = RailMovementCalculator.MoveNext(locomotive.FrontBogie.RailPosition, movement);
            var rear = RailMovementCalculator.MoveNext(locomotive.RearBogie.RailPosition, movement);
            locomotive.SetPosition(front.Position, rear.Position);
            base.SetPosition(locomotive.FrontBogie.RailPosition);

            // events appears on locomotive
            switch (front.EventType)
            {
                case PostMovementEventType.Derail: Derail(); return;
                case PostMovementEventType.Disappear: Dispose(); return;
            }

            // other cars
            if (carsCount > 1) { 
                for (var i = 1; i < carsCount; i++)
                {
                    var car = _cars[i];
                    front = RailMovementCalculator.MoveNext(car.FrontBogie.RailPosition, movement);
                    rear = RailMovementCalculator.MoveNext(car.RearBogie.RailPosition, movement);
                    car.SetPosition(front.Position, rear.Position);
                }
            }         
        }

        // use only for first-time position
        // because all bogies position depends on first pos
        // that can cause instant rail-change when entering reversed dividing path

        // TODO: there is a problem. If front bogie positions before divider, and rear - after, 
        // rear one can go on other track when moving
        public override void SetPosition(in RailPosition pos)
        {
            base.SetPosition(pos);

            var frontBogiePos = FirstBogiePosition;
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
            _lastBogiePosition = _cars[_cars.Length - 1].RearBogie.RailPosition; 
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
