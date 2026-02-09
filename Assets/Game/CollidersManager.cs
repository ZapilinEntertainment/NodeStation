using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    public class CollidersManager : IDisposable
    {
        private readonly Dictionary<int, IColliderOwner> _colliders = new();

        public int Register(IColliderOwner owner)
        {
            var key = owner.GetColliderId();
            _colliders.Add(key, owner);
            return key;
        }

        public void Unregister(int key)
        {
            _colliders.Remove(key);
        }

        public bool TryIdentifyCollider(int key, out IColliderOwner owner) => 
            _colliders.TryGetValue(key, out owner);

        public bool TryIdentifyColliderAs<T>(int key, out T owner) where T : IColliderOwner
        {
            if (TryIdentifyCollider(key, out var ownerOriginal) && ownerOriginal is T converted)
            {
                owner = converted;
                return true;
            }

            owner = default;
            return false;
        }

        public void Dispose()
        {
            _colliders.Clear();
        }
    }
}
