#pragma warning disable CS8601
using System;
using System.Collections.Generic;
using System.Linq;

namespace Corbel.EventBus
{

    public interface IEventBus { }

    public interface IGeneralEventBus : IEventBus
    {
        IUnRegister Subscribe<T>(Delegate callback);
        IUnRegister Subscribe<T>(Action<T> callback);

        void UnSubscribe<T>(Delegate callback);

        void Publish<T>(T item);

        void Publish<T>();

        void Clear();
    }

    public interface ITypeArgEventBus : IEventBus
    {
        IUnRegister Subscribe<T>(Action callback);

        void UnSubscribe<T>(Action callback);

        void Publish<T>();

        void Clear();
    }

    public class GeneralEventBus : IGeneralEventBus
    {
        protected Dictionary<Type, ICollection<Delegate>> eventMap = new();


        public void Publish<T>(T item)
        {
            Type type = typeof(T);
            if (eventMap.ContainsKey(type))
            {
                ICollection<Delegate> events = eventMap[type];
                foreach (Delegate @event in events)
                {
                    @event.DynamicInvoke(item);
                }
            }
        }

        public void Publish<T>()
        {
            Type type = typeof(T);
            if (eventMap.ContainsKey(type))
            {
                ICollection<Delegate> events = eventMap[type];
                foreach (Delegate @event in events)
                {
                    @event.DynamicInvoke();
                }
            }
        }

        public IUnRegister Subscribe<T>(Delegate callback)
        {
            Type keyType = typeof(T);
            var unRegistration = new UnRegistration(() => UnSubscribe<T>(callback));

            if (eventMap.ContainsKey(keyType))
            {
                eventMap[keyType].Add(callback);
            }
            else
            {
                var list = new List<Delegate>(4)
            {
                callback
            };
                eventMap.Add(keyType, list);
            }
            return unRegistration;
        }

        public IUnRegister Subscribe<T>(Action<T> callback)
        {
            return Subscribe<T>(callback as Delegate);
        }

        public void UnSubscribe<T>(Delegate callback)
        {
            var type = typeof(T);
            if (eventMap.ContainsKey(type))
            {
                var events = eventMap[type];
                events.Remove(callback);
            }
        }

        public void UnSubscribe<T>(Action<T> callback)
        {
            UnSubscribe<T>(callback as Delegate);
        }

        public void Clear()
        {
            eventMap.Clear();
        }
    }


    public class TypeArgEventBus : ITypeArgEventBus
    {
        protected Dictionary<Type, Action> eventMap = new();

        public void Publish<T>()
        {
            Type type = typeof(T);
            if (eventMap.ContainsKey(type))
            {
                eventMap[type].Invoke();
            }
        }

        public IUnRegister Subscribe<T>(Action callback)
        {
            var unRegistration = new UnRegistration(() => UnSubscribe<T>(callback));
            Type type = typeof(T);
            if (eventMap.ContainsKey(type))
            {
                eventMap[type] += callback;
            }
            else
            {
                eventMap[type] = callback;
            }
            return unRegistration;
        }

        public void UnSubscribe<T>(Action callback)
        {
            Type type = typeof(T);
            if (eventMap.ContainsKey(type))
            {
                eventMap[type] -= callback;
            }
        }

        public void Clear()
        {
            eventMap.Clear();
        }
    }
}