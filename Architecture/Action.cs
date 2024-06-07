#pragma warning disable CS8618
#nullable enable
using System;
using System.Collections.Generic;

namespace Corbel
{

    public abstract class CorbelAction
    {
        public bool CanDo { get; protected set; }
        public bool HasInit { get; protected set; } = false;
        public PropertyModel? PropertyModel { get; protected set; }
        public Actor Target { get; protected set; }

        public CorbelAction(Actor actor)
        {
            Target = actor;
            PropertyModel = actor.GetPropertyModel();
        }

        public void Refresh(Actor target)
        {
            Target = target;
            PropertyModel = target.GetPropertyModel();
        }

        public void Do()
        {
            if (!HasInit)
            {
                Init();
                HasInit = true;
            }

            if (!CanDo) return;

            OnDo();
        }

        protected abstract void OnDo();

        public abstract void Init();
    }

    public class ActionModel
    {
        protected Dictionary<Type, CorbelAction> actionMap = new();
        public ICollection<CorbelAction> Values => actionMap.Values;

        public void AddProperty<T>(T action) where T : CorbelAction
        {
            actionMap.Add(typeof(T), action);
        }

        public T? Query<T>() where T : CorbelAction
        {
            return actionMap.GetValueOrDefault(typeof(T)) as T;
        }

        public void Remove<T>()
        {
            actionMap.Remove(typeof(T));
        }

        public void Remove(Type type)
        {
            actionMap.Remove(type);
        }

        public void Clear()
        {
            actionMap.Clear();
        }
    }
}