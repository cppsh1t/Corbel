using System;
using System.Collections.Generic;
using System.Linq;

namespace Corbel.EventBus
{

    public class SafeGeneralEventBus : IGeneralEventBus
    {
        protected Dictionary<Type, ICollection<WeakReference<Delegate>>> eventMap = new();


        public void Publish<T>(T item)
        {
            Type type = typeof(T);
            if (eventMap.ContainsKey(type))
            {
                ICollection<WeakReference<Delegate>> events = eventMap[type];
                foreach (WeakReference<Delegate> @event in events)
                {
                    @event.TryGetTarget(out var targetEvent);
                    targetEvent?.DynamicInvoke(item);
                }
            }
        }

        public void Publish<T>()
        {
            Type type = typeof(T);
            if (eventMap.ContainsKey(type))
            {
                ICollection<WeakReference<Delegate>> events = eventMap[type];
                foreach (WeakReference<Delegate> @event in events)
                {
                    @event.TryGetTarget(out var targetEvent);
                    targetEvent?.DynamicInvoke(null);
                }
            }
        }

        public IUnRegister Subscribe<T>(Delegate callback)
        {
            Type keyType = typeof(T);
            UnRegistration unRegistration = new(() => UnSubscribe<T>(callback));
            if (eventMap.ContainsKey(keyType))
            {
                eventMap[keyType].Add(new WeakReference<Delegate>(callback));
            }
            else
            {
                var list = new List<WeakReference<Delegate>>(4)
            {
                new(callback)
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
                var targetEvent = events.FirstOrDefault(weak => { weak.TryGetTarget(out var target); return target == callback; });
                if (targetEvent != null)
                {
                    events.Remove(targetEvent);
                }
            }
        }

        public void Clear()
        {
            eventMap.Clear();
        }

    }
}