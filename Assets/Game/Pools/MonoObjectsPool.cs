using UnityEngine;
using UnityEngine.Pool;

namespace ZE.NodeStation
{
    public class MonoObjectsPool<T> : IObjectPool<T> where T : MonoBehaviour, IPoolable<T>
    {
        private readonly T _prefab;
        private readonly ObjectPool<T> _pool;
        public int CountInactive => _pool.CountActive;

        public MonoObjectsPool(T prefab, int defaultCapacity = 8, int maxCapacity = 128)
        {
            _prefab = prefab;
            _pool = new ObjectPool<T>(
                createFunc: Create,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: OnDestroy,
                defaultCapacity: defaultCapacity,
                maxSize: maxCapacity
                );
        }

        private T Create() 
        {
            var obj = GameObject.Instantiate<T>(_prefab);
            obj.AssignToPool(_pool);
            return obj;
        }

        private void OnGet(T obj) => obj.OnGet();
        private void OnRelease(T obj) => obj.OnRelease();
        private void OnDestroy(T obj) => obj.FinalDispose();

        public T Get() => _pool.Get();

        public PooledObject<T> Get(out T v) => _pool.Get(out v);

        public void Release(T element) => _pool.Release(element);
        public void Clear() => _pool.Clear();
    }
}
