using UnityEngine;
using UnityEngine.Pool;

namespace ZE.NodeStation
{
    public class TrainsTimetableWindow : MonoBehaviour
    {
        private class TrainLinesPool : MonoObjectsPool<TrainTimetableLine>
        {
            private readonly Transform _parent;

            public TrainLinesPool(
                Transform parent,
                TrainTimetableLine prefab, 
                int defaultCapacity = 8, 
                int maxCapacity = 128) : 
                base(prefab, defaultCapacity, maxCapacity)
            {
                _parent = parent;
            }

            // TODO: special effects on appear/disappear

            protected override TrainTimetableLine Create()
            {
                var item =  base.Create();
                item.transform.SetParent(_parent);
                return item;
            }
        }

        [field:SerializeField] public Transform LinesHost { get;private set; }
        [field:SerializeField] public TrainTimetableLine _linePrefab;
        private TrainLinesPool _linesPool;

        public IObjectPool<TrainTimetableLine> GetOrCreateLinesPool()
        {
            if (_linesPool == null) 
                _linesPool = new (LinesHost, _linePrefab, defaultCapacity: 4, maxCapacity: Constants.MAX_TRAIN_ROUTES);

            return _linesPool;
        }       
    }
}
