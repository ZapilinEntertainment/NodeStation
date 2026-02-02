using UnityEngine;

namespace ZE.NodeStation
{
    public class TrainViewFactory
    {
        public ITrainView Build() 
        { 
            var go = new GameObject("train view");
            var view = go.AddComponent<TrainView>();
            return view;
        }
    
    }
}
