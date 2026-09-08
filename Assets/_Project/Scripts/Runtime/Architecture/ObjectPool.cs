using System;
using System.Collections.Generic;
using UnityEngine;

namespace Factura.Gameplay
{
    public interface IPoolable
    {
        void ChangePooledState(bool isPooled);
    }

    public class ObjectPool<T> : IDisposable where T : MonoBehaviour, IPoolable
    {
        private Stack<T> _pool;
        private T _prefab;

        public ObjectPool(T prefab, int preInitInstances = 0)
        {
            _prefab = prefab;
            _pool = new Stack<T>(preInitInstances);

            Initialize(preInitInstances);
        }

        private void Initialize(int count)
        {
            if (count <= 0)
                return;

            for (int i = 0; i < count; i++)
            {
                Add(Create());
            }
        }

        private T Create()
        {
            return UnityEngine.Object.Instantiate(_prefab);
        }

        public void Add(T obj)
        {
            obj.ChangePooledState(isPooled: true);
            _pool.Push(obj);
        }

        public T Get()
        {
            T obj = default;

            if(_pool.Count > 0)
                obj = _pool.Pop();
            else
                obj = Create();

            obj.ChangePooledState(isPooled: false);
            return obj;
        }

        public void Dispose()
        {
            while(_pool.TryPop(out T obj))
            {
                UnityEngine.Object.Destroy(obj.gameObject);
            }
        }
    }
}
