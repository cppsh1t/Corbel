using System.Collections.Generic;
using System.Linq;
using System;

namespace Corbel.Pool
{

    public class MultiObjectPool
    {
        private readonly Dictionary<Type, IObjectPoolBase> poolMap = new();

        public bool Add<T>(IObjectPool<T> pool)
        {
            Type type = typeof(T);
            if (poolMap.ContainsKey(type)) return false;
            poolMap.Add(type, pool);
            return true;
        }

        public bool Remove<T>()
        {
            return poolMap.Remove(typeof(T));
        }

        public T Release<T>()
        {
            Type type = typeof(T);
            if (!poolMap.ContainsKey(type)) return default;

            return (T)poolMap[type].ReleaseNoGeneric();
        }

        public T[] Release<T>(int count)
        {
            Type type = typeof(T);
            if (!poolMap.ContainsKey(type)) return null;

            return poolMap[type].ReleaseNoGeneric(count).Cast<T>().ToArray();
        }

        public bool Recycle<T>(T item)
        {
            Type type = typeof(T);
            if (!poolMap.ContainsKey(type)) return false;

            poolMap[type].RecycleNoGeneric(item!);
            return true;
        }

        public bool Recycle<T>(IEnumerable<T> items)
        {
            Type type = typeof(T);
            if (!poolMap.ContainsKey(type)) return false;
            var noGenItems = items.Cast<object>().ToArray();
            poolMap[type].RecycleNoGeneric(noGenItems);
            return true;
        }

        public void Clear()
        {
            foreach (var pool in poolMap.Values.AsEnumerable())
            {
                pool.Clear();
            }
            poolMap.Clear();
        }


        public void Preload<T>()
        {
            Type type = typeof(T);
            if (poolMap.ContainsKey(type))
            {
                poolMap[type].Preload();
            }
        }

        public void DiposeItems<T>(int count)
        {
            Type type = typeof(T);
            if (poolMap.ContainsKey(type))
            {
                poolMap[type].DiposeItems(count);
            }
        }
    }
}