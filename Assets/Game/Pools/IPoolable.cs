using UnityEngine;
using UnityEngine.Pool;

namespace ZE.NodeStation
{
    public interface IPoolable<T> where T : class
    {
        void AssignToPool(IObjectPool<T> pool);
        void OnGet();
        void OnRelease();
        void FinalDispose();
    }
}
