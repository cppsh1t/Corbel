#pragma warning disable CS8618
#nullable enable
using System;
using Corbel.EventBus;
using Corbel.IOC;

namespace Corbel
{

    public abstract class Regulator : IRegulator
    {
        protected Regulator superior;
        protected IContainer container;
        protected IGeneralEventBus eventBus;
        protected static Regulator Root => CorbelRoot.Instance;

        public Regulator()
        {
            Init();
        }

        protected virtual void Init()
        {
            container = new Container();
            eventBus = new GeneralEventBus();
        }

        public void GetPublish<T>(T item, SearchMode searchMode = SearchMode.Propagation)
        {
            if (searchMode == SearchMode.Local)
            {
                eventBus.Publish(item);
            }
            else if (searchMode == SearchMode.Root)
            {
                Root.eventBus.Publish(item);
            }
            else
            {
                eventBus.Publish(item);
                Root.eventBus.Publish(item);
            }

        }

        public void GetPublish<T>(SearchMode searchMode = SearchMode.Propagation)
        {
            if (searchMode == SearchMode.Local)
            {
                eventBus.Publish<T>();
            }
            else if (searchMode == SearchMode.Root)
            {
                Root.eventBus.Publish<T>();
            }
            else
            {
                eventBus.Publish<T>();
                Root.eventBus.Publish<T>();
            }
        }

        public IUnRegister GetSubscribe<T>(Delegate callback, SearchMode searchMode = SearchMode.Propagation)
        {
            if (searchMode == SearchMode.Local)
            {
                return eventBus.Subscribe<T>(callback);
            }
            else if (searchMode == SearchMode.Root)
            {
                return Root.eventBus.Subscribe<T>(callback);
            }
            else
            {
                SetUnRegistration unRegister = new(eventBus.Subscribe<T>(callback));
                unRegister.AddIUnRegister(eventBus.Subscribe<T>(callback));
                unRegister.AddIUnRegister(Root.eventBus.Subscribe<T>(callback));
                return unRegister;
            }
        }
        public IUnRegister GetSubscribe<T>(Action<T> callback, SearchMode searchMode = SearchMode.Propagation)
        {
            return GetSubscribe<T>(callback as Delegate, searchMode);
        }

        public T? GetResolve<T>(SearchMode searchMode = SearchMode.Propagation)
        {
            if (searchMode == SearchMode.Local)
            {
                return container.Resolve<T>();
            }
            else if (searchMode == SearchMode.Root)
            {
                return Root.container.Resolve<T>();
            }
            else
            {
                return container.Resolve<T>() ?? Root.container.Resolve<T>();
            }

        }

        public object? GetResolve(Type type, SearchMode searchMode = SearchMode.Propagation)
        {
            if (searchMode == SearchMode.Local)
            {
                return container.Resolve(type);
            }
            else if (searchMode == SearchMode.Root)
            {
                return Root.container.Resolve(type);
            }
            else
            {
                return container.Resolve(type) ?? Root.container.Resolve(type);
            }
        }

        public object? GetResolve(string name, SearchMode searchMode = SearchMode.Propagation)
        {
            if (searchMode == SearchMode.Local)
            {
                return container.Resolve(name);
            }
            else if (searchMode == SearchMode.Root)
            {
                return Root.container.Resolve(name);
            }
            else
            {
                return container.Resolve(name) ?? Root.container.Resolve(name);
            }
        }

        public bool GetInject(object obj, SearchMode searchMode = SearchMode.Propagation)
        {
            if (searchMode == SearchMode.Local)
            {
                return container.Inject(obj);
            }
            else if (searchMode == SearchMode.Root)
            {
                return Root.container.Inject(obj);
            }

            else
            {
                if (!container.Inject(obj))
                {
                    return Root.container.Inject(obj);
                }

                return true;
            }
        }

        public Regulator GetRegulator()
        {
            return CorbelRoot.Instance;
        }
    }



    public partial class CorbelRoot : Regulator
    {
        public static CorbelRoot Instance { get; private set; } = new();

        public static Regulator CurrentSceneRegulator { get; internal set; }
        private CorbelRoot() { }
    }

    public class SceneRegulator : Regulator
    {
        public SceneRegulator() : base()
        {
            CorbelRoot.CurrentSceneRegulator = this;
        }

        protected override void Init()
        {
            base.Init();
        }
    }

}