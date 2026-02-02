using UnityEngine;

namespace ZE.NodeStation
{
    public class TrainViewFactory
    {
        private TrainView _prefab;

        public ITrainView Build() 
        { 
            _prefab ??= Resources.Load<TrainView>("TrainView");
            return GameObject.Instantiate<TrainView>(_prefab);
        }
    
    }
}
