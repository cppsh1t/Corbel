using System;
using System.Collections.Generic;
using System.Linq;

namespace Corbel
{

    public static class EventBusExtension
    {
        public static void Publish<T>(this IEmitter self, T item, SearchMode searchMode = SearchMode.Propagation)
        {
            self.GetRegulator().GetPublish(item, searchMode);
        }

        public static void Publish<T>(this IEmitter self, SearchMode searchMode = SearchMode.Propagation)
        {
            self.GetRegulator().GetPublish<T>(searchMode);
        }

        public static void Subscribe<T>(this IEmitter self, Delegate callback, SearchMode searchMode = SearchMode.Propagation)
        {
            self.GetRegulator().GetSubscribe<T>(callback, searchMode);
        }

        public static void Subscribe<T>(this IEmitter self, Action<T> callback, SearchMode searchMode = SearchMode.Propagation)
        {
            self.GetRegulator().GetSubscribe(callback, searchMode);
        }


    }


    public static class ContainerExtension
    {
        public static T? Resolve<T>(this IBeanQuester self, SearchMode searchMode = SearchMode.Propagation)
        {
            return self.GetRegulator().GetResolve<T>(searchMode);
        }

        public static object? Resolve(this IBeanQuester self, Type type, SearchMode searchMode = SearchMode.Propagation)
        {
            return self.GetRegulator().GetResolve(type, searchMode);
        }

        public static object? Resolve(this IBeanQuester self, string name, SearchMode searchMode = SearchMode.Propagation)
        {
            return self.GetRegulator().GetResolve(name, searchMode);
        }

        public static bool Inject(this IBeanQuester self, object obj, SearchMode searchMode = SearchMode.Propagation)
        {
            return self.GetRegulator().GetInject(obj, searchMode);
        }
    }
}