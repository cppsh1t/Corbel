#nullable enable
using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Corbel.Extension
{

    public static class PrintExtension
    {
        public static void Dump(this object? self)
        {
            if (self == null)
            {
                Debug.Log("the object want to Dump is Null!");
            }
            else
            {
                Debug.Log(self);
            }

        }

        public static void DumpHashCode(this object? self)
        {
            if (self == null)
            {
                Debug.Log("the object want to DumpHashCode is Null!");
            }
            else
            {
                Debug.Log(self.GetHashCode());
            }
        }

        public static void DumpJson(this object? self)
        {
            if (self == null)
            {
                Debug.Log("the object want to DumpJson is Null!");
            }
            else
            {
                // self.ToJson().Dump();
            }

        }

        public static void DumpAll<T>(this IEnumerable<T>? self)
        {
            if (self == null)
            {
                Debug.Log("the IEnumerable want to DumpAll is Null!");
            }
            else if (!self.Any())
            {
                Debug.Log("the IEnumerable want to Dump is Empty!");
            }
            else
            {
                foreach (var item in self)
                {
                    item.Dump();
                }
            }
        }

        public static void DumpJsonAll<T>(this IEnumerable<T>? self)
        {
            if (self == null)
            {
                Debug.Log("the IEnumerable want to DumpJsonAll is Null!");
            }
            else if (!self.Any())
            {
                Debug.Log("the IEnumerable want to DumpJson is Empty!");
            }
            else
            {
                foreach (var item in self)
                {
                    // item?.ToJson().Dump();
                }
            }
        }

        public static void DumpMany(this IList? self)
        {
            if (self == null)
            {
                Debug.Log("the IEnumerable want to DumpMany is Null!");
            }
            else if (self.Count == 0)
            {
                Debug.Log("the IList want to Dump is Empty!");
            }
            else
            {
                object? firstItem = null;
                for (var i = 0; i < 1; i++)
                {
                    firstItem = self[i];
                }
                Type itemType = firstItem!.GetType();

                if (typeof(IList).IsAssignableFrom(itemType))
                {
                    var newList = self.OfType<IList>();
                    foreach (var newItem in newList)
                    {
                        newItem.DumpMany();
                    }
                }
                else
                {
                    foreach (var item in self)
                    {
                        item.Dump();
                    }
                }

            }
        }

        public static void DumpJsonMany(this IList? self)
        {
            if (self == null)
            {
                Debug.Log("the IEnumerable want to DumpJsonMany is Null!");
            }
            else if (self.Count == 0)
            {
                Debug.Log("the IList want to DumpJsonMany is Empty!");
            }
            else
            {
                object? firstItem = null;
                for (var i = 0; i < 1; i++)
                {
                    firstItem = self[i];
                }
                Type itemType = firstItem!.GetType();

                if (typeof(IList).IsAssignableFrom(itemType))
                {
                    var newList = self.OfType<IList>();
                    foreach (var newItem in newList)
                    {
                        newItem.DumpJsonMany();
                    }
                }
                else
                {
                    foreach (var item in self)
                    {
                        item.DumpJson();
                    }
                }

            }
        }
    }
}