using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Corbel.Extension
{

    public static class MapExtension
    {
        public static void Put<T, R>(this Dictionary<T, R> self, T key, R value) where T : notnull
        {
            Put(self, key, value, true);
        }

        public static bool Put<T, R>(this Dictionary<T, R> self, T key, R value, bool canOverride) where T : notnull
        {
            if (self.ContainsKey(key))
            {
                if (canOverride)
                {
                    self[key] = value;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else
            {
                self.Add(key, value);
                return true;
            }
        }

        public static IEnumerable<T> Peek<T>(this IEnumerable<T> self, Action<T> action)
        {
            return self.Select(item =>
            {
                action.Invoke(item);
                return item;
            });
        }

        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (var item in self)
            {
                action(item);
            }
        }

        /// <summary>
        /// 警告: 数组无法使用！
        /// </summary>
        public static void Add<T>(this ICollection<T> self, params T[] values)
        {
            foreach (T value in values)
            {
                self.Add(value);
            }
        }

        /// <summary>
        /// 警告: 数组无法使用！
        /// </summary>
        public static void AddAll<T>(this ICollection<T> self, IEnumerable<T> values)
        {
            foreach (T value in values)
            {
                self.Add(value);
            }
        }

        /// <summary>
        /// 警告: 数组无法使用！
        /// </summary>
        public static void AddAll<T>(this ICollection<T> self, ICollection<T> values)
        {
            foreach (T value in values)
            {
                self.Add(value);
            }
        }

        public static void Update<T>(this T[] self, ICollection<T> target)
        {
            for (int i = 0; i < target.Count; i++)
            {
                var item = target.ElementAt(i);
                self[i] = item;
            }
        }

        public static void PutAll<T, R>(this Dictionary<T, R> self, Dictionary<T, R> other, bool canOverride) where T : notnull
        {
            foreach (var (key, value) in other)
            {
                self.Put(key, value, canOverride);
            }
        }

        public static void PutAll<T, R>(this Dictionary<T, R> self, Dictionary<T, R> other) where T : notnull
        {
            foreach (var (key, value) in other)
            {
                self.Put(key, value);
            }
        }

        public static void Remove<T>(this ICollection<T> self, params T[] values)
        {
            foreach (var value in values)
            {
                self.Remove(value);
            }
        }

        public static void PushAll<T>(this Stack<T> self, params T[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                var item = values[i];
                self.Push(item);
            }
        }

        public static void PushAll<T>(this Stack<T> self, ICollection<T> values)
        {
            foreach (T item in values)
            {
                self.Push(item);
            }
        }

        public static void PushAll<T>(this Stack<T> self, IEnumerable<T> values)
        {
            foreach (var item in values)
            {
                self.Push(item);
            }
        }

        public static ICollection<T> Pop<T>(this Stack<T> self, int count)
        {
            var list = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(self.Pop());
            }
            return list;
        }

    }
}