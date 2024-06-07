#nullable enable

using System.Collections;
using Corbel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Corbel.Structure
{
    public abstract class AbstractDoubleKeyMap<T, R, U> where T : notnull where R : notnull
    {

        protected Dictionary<T, U> firstKeyMap = new();
        protected Dictionary<R, U> secondKeyMap = new();
        protected Dictionary<object, object> relateMap = new();

        public IReadOnlyCollection<U> Values => firstKeyMap.Values.Concat(secondKeyMap.Values).ToArray();

        public int Count => firstKeyMap.Count + secondKeyMap.Count;

        public void Clear()
        {
            firstKeyMap.Clear();
            secondKeyMap.Clear();
        }

        public bool Contains(KeyValuePair<T, U> item)
        {
            return firstKeyMap.Contains(item);
        }

        public bool Contains(KeyValuePair<R, U> item)
        {
            return secondKeyMap.Contains(item);
        }

        public bool ContainsKey(T firstKey)
        {
            return firstKeyMap.ContainsKey(firstKey);
        }

        public bool ContainsKey(R secondKey)
        {
            return secondKeyMap.ContainsKey(secondKey);
        }

        public void Put(T? firstKey, R? secondKey, U value)
        {
            if (firstKey != null) firstKeyMap[firstKey] = value;
            if (secondKey != null) secondKeyMap[secondKey] = value;
            if (firstKey != null && secondKey != null)
            {
                relateMap[firstKey] = secondKey;
                relateMap[secondKey] = firstKey;
            }
        }

        public bool Remove(T firstKey)
        {
            bool result = firstKeyMap.Remove(firstKey);
            if (result && relateMap.ContainsKey(firstKey))
            {
                R secondKey = (R)relateMap[firstKey];
                secondKeyMap.Remove(secondKey);
                relateMap.Remove(firstKey);
                relateMap.Remove(secondKey);
            }
            return result;
        }

        public bool Remove(R secondKey)
        {
            bool result = secondKeyMap.Remove(secondKey);
            if (result && relateMap.ContainsKey(secondKey))
            {
                T firstKey = (T)relateMap[secondKey];
                secondKeyMap.Remove(secondKey);
                relateMap.Remove(secondKey);
                relateMap.Remove(firstKey);
            }
            return result;
        }

        public bool Remove(T? firstKey, R? secondKey)
        {
            if (firstKey != null)
            {
                return Remove(firstKey);
            }
            if (secondKey != null)
            {
                return Remove(secondKey);
            }
            return false;
        }

        protected U? GetAndChoose(T firstKey, R secondKey, bool needCompleteKey)
        {
            return needCompleteKey ? ForceGet(firstKey, secondKey) : WeakGet(firstKey, secondKey);
        }

        private U? ForceGet(T firstKey, R secondKey)
        {
            return firstKeyMap.GetValueOrDefault(firstKey) ?? secondKeyMap.GetValueOrDefault(secondKey);
        }

        private U? WeakGet(T? firstKey, R? secondKey)
        {
            if (firstKey != null)
            {
                U? result = firstKeyMap[firstKey];
                if (result != null) return result;
                if (secondKey != null)
                {
                    return secondKeyMap[secondKey] ?? default;
                }
            }

            if (secondKey != null)
            {
                return secondKeyMap[secondKey] ?? default;
            }

            return default;
        }
    }


    public class DoubelKeyMap<T, R, U> : AbstractDoubleKeyMap<T, R, U> where T : notnull where R : notnull
    {
        public U? Get(T firstKey, R secondKey, bool needCompleteKey)
        {
            return GetAndChoose(firstKey, secondKey, needCompleteKey);
        }
    }

    public class SingleQueryMap<T, R, U> : AbstractDoubleKeyMap<T, R, U> where T : notnull where R : notnull
    {
        public U? Get(T? firstKey, R? secondKey)
        {
            return GetAndChoose(firstKey!, secondKey!, false);
        }
    }

    public class MultiQueryMap<T, R, U> : AbstractDoubleKeyMap<T, R, U> where T : notnull where R : notnull
    {
        public U? Get(T firstKey, R secondKey)
        {
            return GetAndChoose(firstKey, secondKey, true);
        }
    }
}